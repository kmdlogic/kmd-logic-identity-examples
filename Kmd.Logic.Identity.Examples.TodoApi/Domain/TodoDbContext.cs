using Microsoft.EntityFrameworkCore;

namespace Kmd.Logic.Identity.Examples.TodoApi.Domain
{
    public class TodoDbContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=todo-api.db");
        }
    }
}