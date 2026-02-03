using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewServicesAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "CarRentals",
                columns: table => new
                {
                    CarRentalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    PricePerDay = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentals", x => x.CarRentalId);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Airline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.FlightId);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerNight = table.Column<double>(type: "float", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.HotelId);
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "ActivityId", "Description", "ImageUrl", "Location", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Văn hóa", "https://images.unsplash.com/photo-1605658639202-0e8666049e29?auto=format&fit=crop&w=800&q=80", "Hội An", "Dạo thuyền trên sông Hoài", 150000.0 },
                    { 2, "Mạo hiểm", "https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=800&q=80", "Hội An", "Lặn ngắm san hô Cù Lao Chàm", 650000.0 },
                    { 3, "Nghệ thuật", "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?auto=format&fit=crop&w=800&q=80", "Hà Nội", "Show múa rối nước Thăng Long", 200000.0 }
                });

            migrationBuilder.InsertData(
                table: "CarRentals",
                columns: new[] { "CarRentalId", "ImageUrl", "Model", "PricePerDay", "Rating", "Seats", "Type" },
                values: new object[,]
                {
                    { 1, "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=800&q=80", "Toyota Fortuner 2023", 1500000.0, 4.7999999999999998, 7, "SUV" },
                    { 2, "https://images.unsplash.com/photo-1503376780353-7e6692767b70?auto=format&fit=crop&w=800&q=80", "VinFast Lux A2.0", 1200000.0, 4.9000000000000004, 5, "Sedan" },
                    { 3, "https://images.unsplash.com/photo-1532581291347-9c39cf10a73c?auto=format&fit=crop&w=800&q=80", "Ford Transit", 2500000.0, 4.7000000000000002, 16, "Van" }
                });

            migrationBuilder.InsertData(
                table: "Flights",
                columns: new[] { "FlightId", "Airline", "ArrivalTime", "Class", "DepartureTime", "FromCode", "Price", "ToCode" },
                values: new object[,]
                {
                    { 1, "Vietnam Airlines", "09:20", "Economy", "08:00", "HAN", 1200000.0, "DAD" },
                    { 2, "Bamboo Airways", "11:30", "Economy", "10:30", "SGN", 950000.0, "PQC" },
                    { 3, "Vietjet Air", "16:15", "Economy", "14:00", "HAN", 780000.0, "SGN" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "HotelId", "Description", "ImageUrl", "Location", "Name", "PricePerNight", "Rating" },
                values: new object[,]
                {
                    { 1, "Bể bơi vô cực, Spa, Nhà hàng 5 sao", "https://images.unsplash.com/photo-1566073771259-6a8506099945?auto=format&fit=crop&w=800&q=80", "Bán đảo Sơn Trà, Đà Nẵng", "InterContinental Danang Sun Peninsula Resort", 12500000.0, 5.0 },
                    { 2, "Kiến trúc độc bản, Biển riêng, Kids Club", "https://images.unsplash.com/photo-1542314831-068cd1dbfeeb?auto=format&fit=crop&w=800&q=80", "Khem Beach, Phú Quốc", "JW Marriott Phu Quoc Emerald Bay Resort", 8900000.0, 5.0 },
                    { 3, "Lịch sử, Trung tâm, Pháp cổ", "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?auto=format&fit=crop&w=800&q=80", "Hoàn Kiếm, Hà Nội", "Sofitel Legend Metropole Hanoi", 6500000.0, 5.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "CarRentals");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "OrderItems");
        }
    }
}
