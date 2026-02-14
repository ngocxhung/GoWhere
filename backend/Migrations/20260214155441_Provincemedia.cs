using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class Provincemedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "ProvinceMedia",
                columns: table => new
                {
                    ProvinceMediaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvinceMedia", x => x.ProvinceMediaId);
                    table.ForeignKey(
                        name: "FK_ProvinceMedia_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId");
                });

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
                value: "https://vcdn1-duc-lich.vnecdn.net/2023/03/01/bai-bien-my-khe-jpeg-167763815-6011-1898-1677638252.jpg?w=1200&h=0&q=100&dpr=1&fit=crop&s=NeyKbOipMzANDXNICpGlRA");

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

          


            migrationBuilder.CreateIndex(
                name: "IX_ProvinceMedia_ProvinceId",
                table: "ProvinceMedia",
                column: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIScheduleHistories");

            migrationBuilder.DropTable(
                name: "ProvinceMedia");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/e/e0/H%E1%BB%93_Ho%C3%A0n_Ki%E1%BA%BFm_-_Lake_of_the_Restored_Sword.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/8/82/Van_Mieu_Hanoi_April_2016.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c8/C%E1%BB%95ng_%C4%90oan_M%C3%B4n_-_Ho%C3%A0ng_th%C3%A0nh_Th%C4%83ng_Long.jpg/2560px-C%E1%BB%95ng_%C4%90oan_M%C3%B4n_-_Ho%C3%A0ng_th%C3%A0nh_Th%C4%83ng_Long.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/thumb/1/1d/L%C4%83ng_Ch%E1%BB%A7_t%E1%BB%8Bch_H%E1%BB%93_Ch%C3%AD_Minh.jpg/2560px-L%C4%83ng_Ch%E1%BB%A7_t%E1%BB%8Bch_H%E1%BB%93_Ch%C3%AD_Minh.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1524231757912-21f4fe3a7200?auto=format&fit=crop&w=1600&q=80");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://media.vneconomy.vn/images/upload/2022/04/28/vneconomy-sun-world-ha-long-complex.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://statics.vinpearl.com/dinh-fansipan-2_1665484834.JPG");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://statics.vinpearl.com/ban-cat-cat-sapa-1_1629275464.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/e/e2/Trang_An_Landscape_Complex.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/8/8b/Bai_Dinh_Pagoda.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 11,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1559592442-7e182c9c6f10?auto=format&fit=crop&w=1600&q=80");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 12,
                column: "ImageUrl",
                value: "https://statics.vinpearl.com/bien-my-khe-da-nang-1_1629853961.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/1/11/Marble_Mountains_Danang.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 14,
                column: "ImageUrl",
                value: "https://images.unsplash.com/photo-1555921015-5532091f6026?auto=format&fit=crop&w=1600&q=80");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 15,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/0/02/Independence_Palace_Saigon.jpg");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 16,
                column: "ImageUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/9/91/Ben_Thanh_Market_Saigon.jpg");
        }
    }
}
