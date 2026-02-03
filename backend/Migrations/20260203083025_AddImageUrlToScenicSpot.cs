using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToScenicSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ScenicSpots",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://statics.vinpearl.com/ho-hoan-kiem-21_1688885249.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/3/39/Hanoi_Temple_of_Literature_%28cropped%29.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://booking.muongthanh.com/upload_images/images/2024%20-%20Nh/H%C3%A0/thoi-diem-du-lich-hoang-thanh.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://tuyengiao.hungyen.dcs.vn/ckfinder/userfiles/files/74176_lang_chu_tich_ho_chi_minh.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://tructhang.vn/wp-content/uploads/2022/08/Vinh-Ha-Long.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/15/67/f6/b7/koi-bridge.jpg?w=800&h=-1&s=1");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://cdn.nhandan.vn/images/1ef398c4e2fb4bf07980a2ded785b3ef6da51f0c0ad991901283c66f347bc9e41cdc313d33455da4eac4bf27a267b2d27b2ed0a2975eea26f660b08a23e7a81b8de59aec459e38ff71c8c3d5fb8fe5eb/f56e1b4cd551030f5a40-5896.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://sun-ecommerce-cdn.azureedge.net/ecommerce/service-sites/asset/SunWorldFansipan/google-doc/post_id_11991/AD_4nXcFbr9S0kZOTux5ZTU1MFeKboEVk3E1eVkj4oo7orF3nztmJHJ43YOtWfUF_KYGVg8MRlPbPOxbKIeNa3VzW8JrWiFTXWF3iTfXCePgXxjldnIzFz1O_MvavorG1XFWhGpVffCYa3YvNP-5rHbB2-fRGmE.webp");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://ik.imagekit.io/tvlk/blog/2022/11/khu-du-lich-trang-an-2.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLGWPCIleB2i8_vXi0hdfoY8VYdTZClezzcQ&s");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 11,
                column: "ImageUrl",
                value: "https://asiaholiday.com.vn/pic/Tour/du-lich-da-nang-ba-na-hills_4865_HasThumb.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 12,
                column: "ImageUrl",
                value: "https://vcdn1-dulich.vnecdn.net/2023/03/01/bai-bien-my-khe-jpeg-167763815-6011-1898-1677638252.jpg?w=1200&h=0&q=100&dpr=1&fit=crop&s=NeyKbOipMzANDXNICpGlRA");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://danangfantasticity.com/wp-content/uploads/2025/08/danh-thang-ngu-hanh-son-da-nang-002.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 14,
                column: "ImageUrl",
                value: "https://daivietourist.vn/wp-content/uploads/2025/05/gioi-thieu-ve-pho-co-hoi-an-8.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 15,
                column: "ImageUrl",
                value: "https://cdn.vntrip.vn/cam-nang/wp-content/uploads/2017/09/2-8.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/9/91/Ben_Thanh_market_2.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ScenicSpots");
        }
    }
}
