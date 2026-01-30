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
        public DbSet<ScenicSpot> ScenicSpots { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<CustomerSchedule> CustomerSchedules { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
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
            modelBuilder.Entity<Province>().HasData(
                // --- TÂY BẮC BỘ (6) ---
                new Province { ProvinceId = 1, ProvinceName = "Lào Cai" },
                new Province { ProvinceId = 2, ProvinceName = "Yên Bái" },
                new Province { ProvinceId = 3, ProvinceName = "Điện Biên" },
                new Province { ProvinceId = 4, ProvinceName = "Hòa Bình" },
                new Province { ProvinceId = 5, ProvinceName = "Lai Châu" },
                new Province { ProvinceId = 6, ProvinceName = "Sơn La" },

                // --- ĐÔNG BẮC BỘ (9) ---
                new Province { ProvinceId = 7, ProvinceName = "Hà Giang" },
                new Province { ProvinceId = 8, ProvinceName = "Cao Bằng" },
                new Province { ProvinceId = 9, ProvinceName = "Bắc Kạn" },
                new Province { ProvinceId = 10, ProvinceName = "Lạng Sơn" },
                new Province { ProvinceId = 11, ProvinceName = "Tuyên Quang" },
                new Province { ProvinceId = 12, ProvinceName = "Thái Nguyên" },
                new Province { ProvinceId = 13, ProvinceName = "Phú Thọ" },
                new Province { ProvinceId = 14, ProvinceName = "Bắc Giang" },
                new Province { ProvinceId = 15, ProvinceName = "Quảng Ninh" },

                // --- ĐỒNG BẰNG SÔNG HỒNG (10) ---
                new Province { ProvinceId = 16, ProvinceName = "Hà Nội" },
                new Province { ProvinceId = 17, ProvinceName = "Hải Phòng" },
                new Province { ProvinceId = 18, ProvinceName = "Bắc Ninh" },
                new Province { ProvinceId = 19, ProvinceName = "Hà Nam" },
                new Province { ProvinceId = 20, ProvinceName = "Hải Dương" },
                new Province { ProvinceId = 21, ProvinceName = "Hưng Yên" },
                new Province { ProvinceId = 22, ProvinceName = "Nam Định" },
                new Province { ProvinceId = 23, ProvinceName = "Ninh Bình" },
                new Province { ProvinceId = 24, ProvinceName = "Thái Bình" },
                new Province { ProvinceId = 25, ProvinceName = "Vĩnh Phúc" },

                // --- BẮC TRUNG BỘ (6) ---
                new Province { ProvinceId = 26, ProvinceName = "Thanh Hóa" },
                new Province { ProvinceId = 27, ProvinceName = "Nghệ An" },
                new Province { ProvinceId = 28, ProvinceName = "Hà Tĩnh" },
                new Province { ProvinceId = 29, ProvinceName = "Quảng Bình" },
                new Province { ProvinceId = 30, ProvinceName = "Quảng Trị" },
                new Province { ProvinceId = 31, ProvinceName = "Thừa Thiên Huế" },

                // --- NAM TRUNG BỘ (8) ---
                new Province { ProvinceId = 32, ProvinceName = "Đà Nẵng" },
                new Province { ProvinceId = 33, ProvinceName = "Quảng Nam" },
                new Province { ProvinceId = 34, ProvinceName = "Quảng Ngãi" },
                new Province { ProvinceId = 35, ProvinceName = "Bình Định" },
                new Province { ProvinceId = 36, ProvinceName = "Phú Yên" },
                new Province { ProvinceId = 37, ProvinceName = "Khánh Hòa" },
                new Province { ProvinceId = 38, ProvinceName = "Ninh Thuận" },
                new Province { ProvinceId = 39, ProvinceName = "Bình Thuận" },

                // --- TÂY NGUYÊN (5) ---
                new Province { ProvinceId = 40, ProvinceName = "Kon Tum" },
                new Province { ProvinceId = 41, ProvinceName = "Gia Lai" },
                new Province { ProvinceId = 42, ProvinceName = "Đắk Lắk" },
                new Province { ProvinceId = 43, ProvinceName = "Đắk Nông" },
                new Province { ProvinceId = 44, ProvinceName = "Lâm Đồng" },

                // --- ĐÔNG NAM BỘ (6) ---
                new Province { ProvinceId = 45, ProvinceName = "TP. Hồ Chí Minh" },
                new Province { ProvinceId = 46, ProvinceName = "Bà Rịa - Vũng Tàu" },
                new Province { ProvinceId = 47, ProvinceName = "Bình Dương" },
                new Province { ProvinceId = 48, ProvinceName = "Bình Phước" },
                new Province { ProvinceId = 49, ProvinceName = "Đồng Nai" },
                new Province { ProvinceId = 50, ProvinceName = "Tây Ninh" },

                // --- ĐỒNG BẰNG SÔNG CỬU LONG (13) ---
                new Province { ProvinceId = 51, ProvinceName = "Cần Thơ" },
                new Province { ProvinceId = 52, ProvinceName = "An Giang" },
                new Province { ProvinceId = 53, ProvinceName = "Bạc Liêu" },
                new Province { ProvinceId = 54, ProvinceName = "Bến Tre" },
                new Province { ProvinceId = 55, ProvinceName = "Cà Mau" },
                new Province { ProvinceId = 56, ProvinceName = "Đồng Tháp" },
                new Province { ProvinceId = 57, ProvinceName = "Hậu Giang" },
                new Province { ProvinceId = 58, ProvinceName = "Kiên Giang" },
                new Province { ProvinceId = 59, ProvinceName = "Long An" },
                new Province { ProvinceId = 60, ProvinceName = "Sóc Trăng" },
                new Province { ProvinceId = 61, ProvinceName = "Tiền Giang" },
                new Province { ProvinceId = 62, ProvinceName = "Trà Vinh" },
                new Province { ProvinceId = 63, ProvinceName = "Vĩnh Long" }
            );
        }
    }
}
