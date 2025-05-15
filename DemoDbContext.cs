using Microsoft.EntityFrameworkCore;
using demo.Models;

namespace demo
{
    public class DemoDbContext : DbContext
    {
        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
