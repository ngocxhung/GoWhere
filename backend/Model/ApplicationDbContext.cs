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
        public DbSet<AISuggestionRequest> AISuggestionRequests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<CarRental> CarRentals { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<AIScheduleHistory> AIScheduleHistories { get; set; }
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

            // --- SEED SCENIC SPOTS ---
            modelBuilder.Entity<ScenicSpot>().HasData(
                // Hà Nội (16) - IsViewed = true (đã duyệt)
                new ScenicSpot { ScenicSpotId = 1, ScenicSpotName = "Hồ Hoàn Kiếm", Description = "Trái tim của thủ đô, biểu tượng văn hóa và lịch sử lâu đời của Hà Nội.", EstimatedVisitPrice = 0, ProvinceId = 16, IsViewed = true, ImageUrl = "https://statics.vinpearl.com/ho-hoan-kiem-21_1688885249.jpg" },
                new ScenicSpot { ScenicSpotId = 2, ScenicSpotName = "Văn Miếu - Quốc Tử Giám", Description = "Trường đại học đầu tiên của Việt Nam, nơi lưu giữ tinh hoa đạo học.", EstimatedVisitPrice = 30000, ProvinceId = 16, IsViewed = true, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/3/39/Hanoi_Temple_of_Literature_%28cropped%29.jpg" },
                new ScenicSpot { ScenicSpotId = 3, ScenicSpotName = "Hoàng Thành Thăng Long", Description = "Quần thể di tích gắn liền với lịch sử kinh thành Thăng Long - Đông Kinh.", EstimatedVisitPrice = 30000, ProvinceId = 16, IsViewed = true, ImageUrl = "https://booking.muongthanh.com/upload_images/images/2024%20-%20Nh/H%C3%A0/thoi-diem-du-lich-hoang-thanh.jpg" },
                new ScenicSpot { ScenicSpotId = 4, ScenicSpotName = "Lăng Chủ tịch Hồ Chí Minh", Description = "Nơi an nghỉ vĩnh hằng của Bác Hồ, vị lãnh tụ kính yêu của dân tộc.", EstimatedVisitPrice = 0, ProvinceId = 16, IsViewed = true, ImageUrl = "https://tuyengiao.hungyen.dcs.vn/ckfinder/userfiles/files/74176_lang_chu_tich_ho_chi_minh.jpg" },
                
                // Quảng Ninh (15) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 5, ScenicSpotName = "Vịnh Hạ Long", Description = "Di sản thiên nhiên thế giới được UNESCO công nhận, nổi tiếng với hàng nghìn đảo đá vôi.", EstimatedVisitPrice = 290000, ProvinceId = 15, IsViewed = true, ImageUrl = "https://tructhang.vn/wp-content/uploads/2022/08/Vinh-Ha-Long.jpg" },
                new ScenicSpot { ScenicSpotId = 6, ScenicSpotName = "Sun World Halong Complex", Description = "Tổ hợp vui chơi giải trí đẳng cấp quốc tế với cáp treo Nữ Hoàng và nhiều trò chơi hấp dẫn.", EstimatedVisitPrice = 700000, ProvinceId = 15, IsViewed = true, ImageUrl = "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/15/67/f6/b7/koi-bridge.jpg?w=800&h=-1&s=1" },
                
                // Lào Cai (1) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 7, ScenicSpotName = "Đỉnh Fansipan", Description = "Nóc nhà Đông Dương, điểm đến lý tưởng cho người yêu thích chinh phục độ cao.", EstimatedVisitPrice = 800000, ProvinceId = 1, IsViewed = true, ImageUrl = "https://cdn.nhandan.vn/images/1ef398c4e2fb4bf07980a2ded785b3ef6da51f0c0ad991901283c66f347bc9e41cdc313d33455da4eac4bf27a267b2d27b2ed0a2975eea26f660b08a23e7a81b8de59aec459e38ff71c8c3d5fb8fe5eb/f56e1b4cd551030f5a40-5896.jpg" },
                new ScenicSpot { ScenicSpotId = 8, ScenicSpotName = "Bản Cát Cát", Description = "Ngôi làng cổ đẹp như tranh của người H'Mông giữa núi rừng Tây Bắc.", EstimatedVisitPrice = 70000, ProvinceId = 1, IsViewed = true, ImageUrl = "https://sun-ecommerce-cdn.azureedge.net/ecommerce/service-sites/asset/SunWorldFansipan/google-doc/post_id_11991/AD_4nXcFbr9S0kZOTux5ZTU1MFeKboEVk3E1eVkj4oo7orF3nztmJHJ43YOtWfUF_KYGVg8MRlPbPOxbKIeNa3VzW8JrWiFTXWF3iTfXCePgXxjldnIzFz1O_MvavorG1XFWhGpVffCYa3YvNP-5rHbB2-fRGmE.webp" },
                
                // Ninh Bình (23) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 9, ScenicSpotName = "Khu du lịch Tràng An", Description = "Di sản văn hóa và thiên nhiên thế giới, nổi tiếng với hệ thống hang động và sông nước.", EstimatedVisitPrice = 250000, ProvinceId = 23, IsViewed = true, ImageUrl = "https://ik.imagekit.io/tvlk/blog/2022/11/khu-du-lich-trang-an-2.jpg" },
                new ScenicSpot { ScenicSpotId = 10, ScenicSpotName = "Chùa Bái Đính", Description = "Ngôi chùa lớn nhất Đông Nam Á, sở hữu nhiều kỷ lục châu Á và Việt Nam.", EstimatedVisitPrice = 0, ProvinceId = 23, IsViewed = true, ImageUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLGWPCIleB2i8_vXi0hdfoY8VYdTZClezzcQ&s" },
                
                // Đà Nẵng (32) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 11, ScenicSpotName = "Bà Nà Hills", Description = "Đường lên tiên cảnh với Cầu Vàng nổi tiếng thế giới và làng Pháp cổ kính.", EstimatedVisitPrice = 850000, ProvinceId = 32, IsViewed = true, ImageUrl = "https://asiaholiday.com.vn/pic/Tour/du-lich-da-nang-ba-na-hills_4865_HasThumb.jpg" },
                new ScenicSpot { ScenicSpotId = 12, ScenicSpotName = "Biển Mỹ Khê", Description = "Được tạp chí Forbes bình chọn là một trong những bãi biển quyến rũ nhất hành tinh.", EstimatedVisitPrice = 0, ProvinceId = 32, IsViewed = true, ImageUrl = "https://vcdn1-duc-lich.vnecdn.net/2023/03/01/bai-bien-my-khe-jpeg-167763815-6011-1898-1677638252.jpg?w=1200&h=0&q=100&dpr=1&fit=crop&s=NeyKbOipMzANDXNICpGlRA" },
                new ScenicSpot { ScenicSpotId = 13, ScenicSpotName = "Ngũ Hành Sơn", Description = "Quần thể 5 ngọn núi đá vôi với nhiều hang động và chùa chiền linh thiêng.", EstimatedVisitPrice = 40000, ProvinceId = 32, IsViewed = true, ImageUrl = "https://danangfantasticity.com/wp-content/uploads/2025/08/danh-thang-ngu-hanh-son-da-nang-002.jpg" },
                
                // Quảng Nam (33) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 14, ScenicSpotName = "Phố cổ Hội An", Description = "Di sản văn hóa thế giới, nơi lưu giữ vẻ đẹp cổ kính và yên bình bên dòng sông Hoài.", EstimatedVisitPrice = 0, ProvinceId = 33, IsViewed = true, ImageUrl = "https://daivietourist.vn/wp-content/uploads/2025/05/gioi-thieu-ve-pho-co-hoi-an-8.jpg" },
                
                // TP. Hồ Chí Minh (45) - IsViewed = true
                new ScenicSpot { ScenicSpotId = 15, ScenicSpotName = "Dinh Độc Lập", Description = "Di tích lịch sử quan trọng, biểu tượng của sự thống nhất đất nước.", EstimatedVisitPrice = 40000, ProvinceId = 45, IsViewed = true, ImageUrl = "https://cdn.vntrip.vn/cam-nang/wp-content/uploads/2017/09/2-8.jpg" },
                new ScenicSpot { ScenicSpotId = 16, ScenicSpotName = "Chợ Bến Thành", Description = "Biểu tượng văn hóa và thương mại lâu đời của Sài Gòn.", EstimatedVisitPrice = 0, ProvinceId = 45, IsViewed = true, ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/91/Ben_Thanh_market_2.jpg" }
            );

            // --- SEED MEDIA ---
            modelBuilder.Entity<Media>().HasData(
                new Media { MediaId = 1, ScenicSpotId = 1, MediaUrl = "/uploads/hanoi/hoankiem.jpg" },
                new Media { MediaId = 2, ScenicSpotId = 2, MediaUrl = "/uploads/hanoi/vanmieu.jpg" },
                new Media { MediaId = 3, ScenicSpotId = 5, MediaUrl = "/uploads/quangninh/halong.jpg" },
                new Media { MediaId = 4, ScenicSpotId = 7, MediaUrl = "/uploads/laocai/fansipan.jpg" },
                new Media { MediaId = 5, ScenicSpotId = 9, MediaUrl = "/uploads/ninhbinh/trangan.jpg" },
                new Media { MediaId = 6, ScenicSpotId = 11, MediaUrl = "/uploads/danang/banahills.jpg" },
                new Media { MediaId = 7, ScenicSpotId = 14, MediaUrl = "/uploads/quangnam/hoian.jpg" },
                new Media { MediaId = 8, ScenicSpotId = 15, MediaUrl = "/uploads/hcm/dinhdoclap.jpg" }
            );

            // --- SEED HOTELS ---
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { HotelId = 1, Name = "InterContinental Danang Sun Peninsula Resort", Location = "Bán đảo Sơn Trà, Đà Nẵng", PricePerNight = 12500000, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80", Description = "Bể bơi vô cực, Spa, Nhà hàng 5 sao", ProvinceId = 32 },
                new Hotel { HotelId = 2, Name = "JW Marriott Phu Quoc Emerald Bay Resort", Location = "Khem Beach, Phú Quốc", PricePerNight = 8900000, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=800&q=80", Description = "Kiến trúc độc bản, Biển riêng, Kids Club", ProvinceId = 58 },
                new Hotel { HotelId = 3, Name = "Sofitel Legend Metropole Hanoi", Location = "Hoàn Kiếm, Hà Nội", PricePerNight = 6500000, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=800&q=80", Description = "Lịch sử, Trung tâm, Pháp cổ", ProvinceId = 16 },
                new Hotel { HotelId = 4, Name = "Lotte Hotel Hanoi", Location = "Ba Đình, Hà Nội", PricePerNight = 3500000, Rating = 5, ImageUrl = "https://images.unsplash.com/photo-1551882547-ff43c61f3630?auto=format&fit=crop&w=800&q=80", Description = "Tòa nhà cao tầng, View toàn thành phố, Sang trọng", ProvinceId = 16 },
                new Hotel { HotelId = 5, Name = "Hotel de l'Opera Hanoi - MGallery", Location = "Hoàn Kiếm, Hà Nội", PricePerNight = 4200000, Rating = 4.5, ImageUrl = "https://images.unsplash.com/photo-1563911517161-073dd8ffb24e?auto=format&fit=crop&w=800&q=80", Description = "Nghệ thuật, Gần Nhà hát Lớn, Đẳng cấp", ProvinceId = 16 }
            );

            // --- SEED FLIGHTS ---
            modelBuilder.Entity<Flight>().HasData(
                new Flight { FlightId = 1, Airline = "Vietnam Airlines", FromCode = "HAN", ToCode = "DAD", DepartureTime = "08:00", ArrivalTime = "09:20", Price = 1200000, Class = "Economy" },
                new Flight { FlightId = 2, Airline = "Bamboo Airways", FromCode = "SGN", ToCode = "PQC", DepartureTime = "10:30", ArrivalTime = "11:30", Price = 950000, Class = "Economy" },
                new Flight { FlightId = 3, Airline = "Vietjet Air", FromCode = "HAN", ToCode = "SGN", DepartureTime = "14:00", ArrivalTime = "16:15", Price = 780000, Class = "Economy" }
            );

            // --- SEED CAR RENTALS ---
            modelBuilder.Entity<CarRental>().HasData(
                new CarRental { CarRentalId = 1, Model = "Toyota Fortuner 2023", Type = "SUV", Seats = 7, PricePerDay = 1500000, Rating = 4.8, ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=800&q=80" },
                new CarRental { CarRentalId = 2, Model = "VinFast Lux A2.0", Type = "Sedan", Seats = 5, PricePerDay = 1200000, Rating = 4.9, ImageUrl = "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=800&q=80" },
                new CarRental { CarRentalId = 3, Model = "Ford Transit", Type = "Van", Seats = 16, PricePerDay = 2500000, Rating = 4.7, ImageUrl = "https://images.unsplash.com/photo-1532581291347-9c39cf10a73c?auto=format&fit=crop&w=800&q=80" }
            );

            // --- SEED ACTIVITIES ---
            modelBuilder.Entity<Activity>().HasData(
                new Activity { ActivityId = 1, Name = "Dạo thuyền trên sông Hoài", Price = 150000, Location = "Hội An", Description = "Văn hóa", ImageUrl = "https://images.unsplash.com/photo-1605658639202-0e8666049e29?auto=format&fit=crop&w=800&q=80" },
                new Activity { ActivityId = 2, Name = "Lặn ngắm san hô Cù Lao Chàm", Price = 650000, Location = "Hội An", Description = "Mạo hiểm", ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=800&q=80" },
                new Activity { ActivityId = 3, Name = "Show múa rối nước Thăng Long", Price = 200000, Location = "Hà Nội", Description = "Nghệ thuật", ImageUrl = "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?auto=format&fit=crop&w=800&q=80" }
            );
        }
    }
}
