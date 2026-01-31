using MovieInfoBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieInfoBackend.Auth;

namespace MovieInfoBackend.Data
{
    public class TempContext : IdentityDbContext<ApplicationUser>
    {
        public TempContext(DbContextOptions<TempContext> options) : base(options)
        {
        }

        public DbSet<TestModel> TestModels { get; set; }
    }
}