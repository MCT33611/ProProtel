using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProPortel.Models;

namespace ProPortel.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : IdentityDbContext(option)
    {
        public DbSet<OTP> OTPs { get; set; }
        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plan>().HasData(
                    new Plan { Id = 1, Name = "Basic", Description = "Basic plan description", Price = 20, CreatedAt = DateTime.Now, DurationMonths = 1 },
                    new Plan { Id = 2, Name = "Premium", Description = "Premium plan description", Price = 50, CreatedAt = DateTime.Now, DurationMonths = 3 },
                    new Plan { Id = 3, Name = "Business", Description = "Premium plan description", Price = 100, CreatedAt = DateTime.Now, DurationMonths = 6 }
                );
        }

    }
}
