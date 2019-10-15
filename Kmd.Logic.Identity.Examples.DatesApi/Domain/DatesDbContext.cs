using Microsoft.EntityFrameworkCore;

namespace Kmd.Logic.Identity.Examples.DatesApi.Domain
{
    public class DatesDbContext : DbContext
    {
        public DbSet<DateDetail> DateDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dates-api.db");
        }
    }
}