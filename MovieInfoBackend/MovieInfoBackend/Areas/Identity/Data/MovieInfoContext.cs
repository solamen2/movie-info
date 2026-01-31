using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieInfoBackend.Auth;

namespace MovieInfoBackend.Areas.Identity.Data;

public class MovieInfoContext : IdentityDbContext<ApplicationUser>
{
    public MovieInfoContext(DbContextOptions<MovieInfoContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        // TODO: Add other things that are needed here, like TestModel for the time being
    }
}
