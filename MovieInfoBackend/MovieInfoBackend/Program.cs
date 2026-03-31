using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.Endpoints;
using Scalar.AspNetCore;
using MovieInfoBackend.Auth;
using MovieInfoBackend.Areas.Identity.Data;
using System.Security.Claims;
using Serilog;
using Polly;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
WebApplication app;

ConfigLogging();
try
{
    Log.Information("Starting app service configuration...");
    AddServices();
    ConfigDatabase();
    ConfigAuth();

    Log.Information("Building app...");
    app = builder.Build();

    Log.Information("Migrating database...");
    MigrateDatabase();
    Log.Information("Setting up app...");
    SetUpApp();

    Log.Information("Mapping endpoints...");
    AuthEndpoints.Map(app);
    MovieEndpoints.Map(app);

    Log.Information("Starting app...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

void AddServices()
{
    // Add non-auth services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder
        .Services
            .AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                    policy.WithOrigins("http://localhost:8080", // TODO: Test that all these work!
                                       "http://localhost:8081",
                                       "https://movie-info-flyio.fly.dev",
                                       "https://movie-info-stage.*",
                                       "https://movie-info-prod.*",
                                       "https://movieinfo.dev");
                    });
            })
            .AddMemoryCache()
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());  // serializes enums as string, not backing int
            })
            .AddHttpClient<MovieHttpClient>()
                .AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(3, retryNumber => TimeSpan.FromMilliseconds(600)))
                .AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
                .SetHandlerLifetime(TimeSpan.FromMinutes(2))  // NOTE: This is the default, but reminds me how to change if needed
            ;

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddOpenApi();
    }
}

void ConfigDatabase()
{
    // Add DB
    string? connectionString;

    if (builder.Environment.IsProduction())
    {
        connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_PROD_DB");  // from environment variable
    }
    else if (builder.Environment.IsStaging())
    {
        connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_STAGE_DB");  // from environment variable
    }
    else  // should be builder.Environment.IsDevelopment()
    {
        switch (ProgramConfig.DbConnType)
        {
            case LocalDbConnType.Local:
                connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_LOCAL_DB");  // from launch.json environment variable
                break;
            case LocalDbConnType.LocalDocker:
                // from .env file (but same as previous with host as "host.docker.internal" instead of "localhost")
                connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_LOCAL_DOCKER_DB");
                break;
            case LocalDbConnType.AzureDev:
                // from .env file using --env-file in Docker, or from launch.json outside of Docker (env var needs to be added if you do this!)
                connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_DEV_DB");
                break;
            default:  // should never happen currently
                throw new ArgumentException("LocalDbConnType must be one of the expected values.");
        }
    }

    builder.Services.AddDbContext<MovieInfoDbContext>(options => options.UseSqlServer(connectionString));
}

void ConfigAuth()
{
    // Authentication
    builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
        .AddIdentityCookies();
    builder.Services.AddIdentityCore<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<MovieInfoDbContext>()
        .AddApiEndpoints();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = ProgramConfig.LoginCookieTimeout;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

    // Authorization
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(ProgramConstants.LoggedInUsersOnlyPolicyName, policy => 
            policy.RequireClaim(ClaimTypes.Role, ProgramConstants.LoggedInUsersOnlyPolicyClaimName));
        options.AddPolicy(ProgramConstants.SearchUsersOnlyPolicyName, policy => 
            policy.RequireClaim(ClaimTypes.Role, ProgramConstants.SearchUsersOnlyPolicyClaimName));
    });
    builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
        AdditionalUserClaimsPrincipalFactory>();
}

void ConfigLogging()
{
    builder.Host.UseSerilog((context, loggerConfig) =>
        loggerConfig.ReadFrom.Configuration(context.Configuration)  // NOTE: from appsettings.json
    );
}

void MigrateDatabase()
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MovieInfoDbContext>();
    db.Database.Migrate();
}

void SetUpApp()
{
    // Set up exception handling and APIs
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        if (ProgramConfig.DbConnType != LocalDbConnType.AzureDev)
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    // Map authN routes
    RouteGroupBuilder apiGroup = app.MapGroup(ProgramConstants.ApiRoutePrefix);
    apiGroup.MapIdentityApi<ApplicationUser>();

    // For React
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("/index.html");

    // Logging
    app.UseSerilogRequestLogging();

    // CORS
    app.UseCors();
}