using Azure.Core;
using Microsoft.EntityFrameworkCore;
using MovieInfoBackend.Data;
using MovieInfoBackend.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add DB

string? connectionString;

if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
{
    connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_PROD_DB");  // from environment variable
}
else
{
    LocalDbConnType localDbConnType = LocalDbConnType.Local;

    switch (localDbConnType)
    {
        case LocalDbConnType.Local:
            connectionString = builder.Configuration.GetConnectionString("MovieInfoLocalDb");
            break;
        case LocalDbConnType.LocalDocker:
            connectionString = builder.Configuration.GetConnectionString("MovieInfoLocalDockerDb");
            break;
        case LocalDbConnType.AzureDev:
            connectionString = builder.Configuration.GetConnectionString("MOVIE_INFO_AZURE_DEV_DB");  // from .env file using --env-file in Docker, or from appsettings.Development.json outside of Docker
            break;
        default:  // should never happen currently
            throw new ArgumentException("LocalDbConnType must be one of the expected values.");
    }
}

builder.Services.AddDbContext<MovieInfoContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// For React
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

WeatherEndpoints.Map(app);
CreateDbIfNotExists(app);

app.Run();

static void CreateDbIfNotExists(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<MovieInfoContext>();
            TempDbInit.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}