using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieInfoBackend.Helpers;
using MovieInfoBackend.Endpoints;
using Scalar.AspNetCore;
using MovieInfoBackend.Auth;
using MovieInfoBackend.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

SetupDatabase();
SetupAuth();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
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

app.UseHttpsRedirection();

WeatherEndpoints.Map(app);

app.Run();

void SetupDatabase()
{
    // Add DB
    string? connectionString;

    if (builder.Environment.IsProduction())
    {
        connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_PROD_DB");  // from environment variable
    }
    else if (builder.Environment.IsStaging())
    {
        connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_DEV_DB");  // from environment variable
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

    builder.Services.AddDbContext<MovieInfoContext>(options => options.UseSqlServer(connectionString));
}

void SetupAuth()
{
    // Set up authN / authZ services
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("LocalAuthIssuer");  // TODO: Add fields to bearer token and validate later
    builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<MovieInfoContext>();
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(ProgramConstants.LoggedInUsersOnlyPolicyName, policy => 
            policy.RequireClaim(ClaimTypes.Role, ProgramConstants.LoggedInUsersOnlyPolicyClaimName));
        options.AddPolicy(ProgramConstants.SearchUsersOnlyPolicyName, policy => 
            policy.RequireClaim(ClaimTypes.Role, ProgramConstants.SearchUsersOnlyPolicyClaimName));
    });
    builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,
        AdditionalUserClaimsPrincipalFactory>();

    // Configure auth cookie settings TODO fix these!
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/LoginAsync"; // Set your login path
        options.LogoutPath = "/LogoutAsync"; // Set your logout path
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = ProgramConfig.LoginCookieTimeout;
    });
}