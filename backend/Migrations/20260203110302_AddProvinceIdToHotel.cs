using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProvinceIdToHotel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProvinceId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 1,
                column: "ProvinceId",
                value: 32);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 2,
                column: "ProvinceId",
                value: 58);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 3,
                column: "ProvinceId",
                value: 16);

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "HotelId", "Description", "ImageUrl", "Location", "Name", "PricePerNight", "ProvinceId", "Rating" },
                values: new object[,]
                {
                    { 4, "Tòa nhà cao tầng, View toàn thành phố, Sang trọng", "https://images.unsplash.com/photo-1551882547-ff43c61f3630?auto=format&fit=crop&w=800&q=80", "Ba Đình, Hà Nội", "Lotte Hotel Hanoi", 3500000.0, 16, 5.0 },
                    { 5, "Nghệ thuật, Gần Nhà hát Lớn, Đẳng cấp", "https://images.unsplash.com/photo-1563911517161-073dd8ffb24e?auto=format&fit=crop&w=800&q=80", "Hoàn Kiếm, Hà Nội", "Hotel de l'Opera Hanoi - MGallery", 4200000.0, 16, 4.5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_ProvinceId",
                table: "Hotels",
                column: "ProvinceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Provinces_ProvinceId",
                table: "Hotels",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Provinces_ProvinceId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_ProvinceId",
                table: "Hotels");

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ProvinceId",
                table: "Hotels");
        }
    }
}
