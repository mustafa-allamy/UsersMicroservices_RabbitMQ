using Microsoft.EntityFrameworkCore;
using Models.Models;
using System;

namespace AuthService
{
    public class AuthContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        public AuthContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql(
                 "Server=localhost;Port=5432;Database=AuthService;User Id=postgres;Password=admin123;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //password is admin123
            modelBuilder.Entity<User>().HasData(new User[] {
                new User
                {
                    Id= Guid.Parse("43bfd053-177b-437b-9843-39a475129c00"),
                    Email= "mustafa@example.com",
                    FullName = "Mustafa Mohammed",
                    Password = "$2a$11$hRrLwaHUMGPPoHWY7W5j4.0nHQT5kiQkkqLWwPOPwJVTp7i8tFwYS",
                    PhoneNumber = "12345",
                    Role = "Admin"
                },
            });
        }

    }
}