using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the User-Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role { RoleId = 1, RoleName = "User" },
                    new Role { RoleId = 2, RoleName = "Admin" }
                );
            modelBuilder.Entity<User>()
                .HasData(
                    new User { UserId = 1, Username = "regularuser", Password = "password123", Email = "regularuser@example.com", FullName = "Regular User", PhoneNumber = "1234567890", RoleId = 1 },
                    new User { UserId = 2, Username = "adminuser", Password = "adminpassword", Email = "adminuser@example.com", FullName = "Admin User", PhoneNumber = "0987654321", RoleId = 2 }
                );
        }
    }
}
