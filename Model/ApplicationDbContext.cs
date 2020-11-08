using Microsoft.EntityFrameworkCore;

namespace JobeSharp.Model
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Run> Runs { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}