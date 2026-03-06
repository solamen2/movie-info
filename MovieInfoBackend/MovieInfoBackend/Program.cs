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
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
WebApplication app;

ConfigLogging();
try
{
    Log.Information("Starting app configuration...");
    AddServices();
    ConfigDatabase();
    ConfigAuth();

    Log.Information("Building app...");
    app = builder.Build();

    Log.Information("Migrating database...");
    MigrateDatabase();
    Log.Information("Setting up web server...");
    SetUpWebServer();

    Log.Information("Mapping endpoints...");
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
                .SetHandlerLifetime(TimeSpan.FromMinutes(2));  // NOTE: This is the default, but reminds me how to change if needed

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
                connectionString = builder.Configuration.GetConnectionString("MovieInfoLocalDb");
                break;
            case LocalDbConnType.LocalDocker:
                connectionString = builder.Configuration.GetConnectionString("MovieInfoLocalDockerDb");
                break;
            case LocalDbConnType.AzureDev:
                // from .env file using --env-file in Docker, or from appsettings.Development.json outside of Docker (env var needs to be added if you do this!)
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
    
    builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<MovieInfoDbContext>();
    builder.Services.AddAuthentication().AddBearerToken();
    builder.Services.ConfigureApplicationCookie(options =>
    {
        // TODO: Maybe revisit these later
        options.LoginPath = "/login"; // Set your login path
        options.LogoutPath = "/logout"; // Set your logout path
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = ProgramConfig.LoginCookieTimeout;
    });

    builder.Services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme).Configure(
        options =>
        {
            options.BearerTokenExpiration = ProgramConfig.LoginCookieTimeout;
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

void SetUpWebServer()
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
    app.MapIdentityApi<ApplicationUser>();

    // For React
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("/index.html");

    // Logging
    app.UseSerilogRequestLogging();
}