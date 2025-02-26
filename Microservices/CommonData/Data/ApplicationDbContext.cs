using Microsoft.EntityFrameworkCore;
using CommonData.Models;

namespace CommonData.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Neighbor> Neighbors { get; set; }
    }
}