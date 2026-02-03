using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScenicSpotImageUrls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 1,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 2,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 3,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 4,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 5,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 6,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 7,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 8,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 9,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 10,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 11,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 12,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 13,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 14,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 15,
                column: "ImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 16,
                column: "ImageUrl",
                value: null);
        }
    }
}
