using FoodTourneyBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodTourneyBack.Controllers
{
    public class ZipContext : DbContext
    {
        public ZipContext(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
        }
        public DbSet<ZipTuple> Zips { get; set; }
        public DbSet<Place> Places { get; set; }
    }
}
