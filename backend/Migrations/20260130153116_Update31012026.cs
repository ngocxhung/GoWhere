using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class Update31012026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    CustomerScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.FeedbackId);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_CustomerSchedules_Feedbacks_FeedbackId",
                        column: x => x.FeedbackId,
                        principalTable: "Feedbacks",
                        principalColumn: "FeedbackId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerSchedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenicSpots",
                columns: table => new
                {
                    ScenicSpotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScenicSpotName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EstimatedVisitPrice = table.Column<double>(type: "float", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenicSpots", x => x.ScenicSpotId);
                    table.ForeignKey(
                        name: "FK_ScenicSpots_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerScheduleScenicSpot",
                columns: table => new
                {
                    CustomerSchedulesScheduleId = table.Column<int>(type: "int", nullable: false),
                    ScenicSpotsScenicSpotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerScheduleScenicSpot", x => new { x.CustomerSchedulesScheduleId, x.ScenicSpotsScenicSpotId });
                    table.ForeignKey(
                        name: "FK_CustomerScheduleScenicSpot_CustomerSchedules_CustomerSchedulesScheduleId",
                        column: x => x.CustomerSchedulesScheduleId,
                        principalTable: "CustomerSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerScheduleScenicSpot_ScenicSpots_ScenicSpotsScenicSpotId",
                        column: x => x.ScenicSpotsScenicSpotId,
                        principalTable: "ScenicSpots",
                        principalColumn: "ScenicSpotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    MediaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScenicSpotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.MediaId);
                    table.ForeignKey(
                        name: "FK_Medias_ScenicSpots_ScenicSpotId",
                        column: x => x.ScenicSpotId,
                        principalTable: "ScenicSpots",
                        principalColumn: "ScenicSpotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "ProvinceId", "ProvinceName" },
                values: new object[,]
                {
                    { 1, "Lào Cai" },
                    { 2, "Yên Bái" },
                    { 3, "Điện Biên" },
                    { 4, "Hòa Bình" },
                    { 5, "Lai Châu" },
                    { 6, "Sơn La" },
                    { 7, "Hà Giang" },
                    { 8, "Cao Bằng" },
                    { 9, "Bắc Kạn" },
                    { 10, "Lạng Sơn" },
                    { 11, "Tuyên Quang" },
                    { 12, "Thái Nguyên" },
                    { 13, "Phú Thọ" },
                    { 14, "Bắc Giang" },
                    { 15, "Quảng Ninh" },
                    { 16, "Hà Nội" },
                    { 17, "Hải Phòng" },
                    { 18, "Bắc Ninh" },
                    { 19, "Hà Nam" },
                    { 20, "Hải Dương" },
                    { 21, "Hưng Yên" },
                    { 22, "Nam Định" },
                    { 23, "Ninh Bình" },
                    { 24, "Thái Bình" },
                    { 25, "Vĩnh Phúc" },
                    { 26, "Thanh Hóa" },
                    { 27, "Nghệ An" },
                    { 28, "Hà Tĩnh" },
                    { 29, "Quảng Bình" },
                    { 30, "Quảng Trị" },
                    { 31, "Thừa Thiên Huế" },
                    { 32, "Đà Nẵng" },
                    { 33, "Quảng Nam" },
                    { 34, "Quảng Ngãi" },
                    { 35, "Bình Định" },
                    { 36, "Phú Yên" },
                    { 37, "Khánh Hòa" },
                    { 38, "Ninh Thuận" },
                    { 39, "Bình Thuận" },
                    { 40, "Kon Tum" },
                    { 41, "Gia Lai" },
                    { 42, "Đắk Lắk" },
                    { 43, "Đắk Nông" },
                    { 44, "Lâm Đồng" },
                    { 45, "TP. Hồ Chí Minh" },
                    { 46, "Bà Rịa - Vũng Tàu" },
                    { 47, "Bình Dương" },
                    { 48, "Bình Phước" },
                    { 49, "Đồng Nai" },
                    { 50, "Tây Ninh" },
                    { 51, "Cần Thơ" },
                    { 52, "An Giang" },
                    { 53, "Bạc Liêu" },
                    { 54, "Bến Tre" },
                    { 55, "Cà Mau" },
                    { 56, "Đồng Tháp" },
                    { 57, "Hậu Giang" },
                    { 58, "Kiên Giang" },
                    { 59, "Long An" },
                    { 60, "Sóc Trăng" },
                    { 61, "Tiền Giang" },
                    { 62, "Trà Vinh" },
                    { 63, "Vĩnh Long" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSchedules_FeedbackId",
                table: "CustomerSchedules",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSchedules_UserId",
                table: "CustomerSchedules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerScheduleScenicSpot_ScenicSpotsScenicSpotId",
                table: "CustomerScheduleScenicSpot",
                column: "ScenicSpotsScenicSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ScenicSpotId",
                table: "Medias",
                column: "ScenicSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenicSpots_ProvinceId",
                table: "ScenicSpots",
                column: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerScheduleScenicSpot");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "CustomerSchedules");

            migrationBuilder.DropTable(
                name: "ScenicSpots");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
