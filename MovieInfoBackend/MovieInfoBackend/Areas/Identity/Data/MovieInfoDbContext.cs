using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MovieInfoBackend.Auth;

namespace MovieInfoBackend.Areas.Identity.Data;

[ExcludeFromCodeCoverage]
public class MovieInfoDbContext : IdentityDbContext<ApplicationUser>
{
    public MovieInfoDbContext(DbContextOptions<MovieInfoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug)));
}
