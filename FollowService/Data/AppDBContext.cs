using FollowService.Model;
using Microsoft.EntityFrameworkCore;

namespace FollowService.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<Follow> follows { get; set; } // Updated to PascalCase for better convention


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=postgres-follow;Port=5434;Database=mydatabase;Username=myuser;Password=mypassword;");
        }
    }
}
