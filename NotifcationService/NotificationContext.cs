using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace NotifcationService
{
    public class NotificationContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }

        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options)
        {
        }
        public NotificationContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql(
                "Server=localhost;Port=5432;Database=NotificationService;User Id=postgres;Password=admin123;");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

    }
}