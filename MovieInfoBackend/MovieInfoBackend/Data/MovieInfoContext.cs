using MovieInfoBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace MovieInfoBackend.Data
{
    public class MovieInfoContext : DbContext
    {
        public MovieInfoContext(DbContextOptions<MovieInfoContext> options) : base(options)
        {
        }

        public DbSet<TestModel> TestModels { get; set; }
    }
}