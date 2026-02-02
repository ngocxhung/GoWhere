using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddScenicSpotAndAIChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AISuggestionRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AISuggestionRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_AISuggestionRequests_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_AISuggestionRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ScenicSpots",
                columns: new[] { "ScenicSpotId", "Description", "EstimatedVisitPrice", "ProvinceId", "ScenicSpotName" },
                values: new object[,]
                {
                    { 1, "Trái tim của thủ đô, biểu tượng văn hóa và lịch sử lâu đời của Hà Nội.", 0.0, 16, "Hồ Hoàn Kiếm" },
                    { 2, "Trường đại học đầu tiên của Việt Nam, nơi lưu giữ tinh hoa đạo học.", 30000.0, 16, "Văn Miếu - Quốc Tử Giám" },
                    { 3, "Quần thể di tích gắn liền với lịch sử kinh thành Thăng Long - Đông Kinh.", 30000.0, 16, "Hoàng Thành Thăng Long" },
                    { 4, "Nơi an nghỉ vĩnh hằng của Bác Hồ, vị lãnh tụ kính yêu của dân tộc.", 0.0, 16, "Lăng Chủ tịch Hồ Chí Minh" },
                    { 5, "Di sản thiên nhiên thế giới được UNESCO công nhận, nổi tiếng với hàng nghìn đảo đá vôi.", 290000.0, 15, "Vịnh Hạ Long" },
                    { 6, "Tổ hợp vui chơi giải trí đẳng cấp quốc tế với cáp treo Nữ Hoàng và nhiều trò chơi hấp dẫn.", 700000.0, 15, "Sun World Halong Complex" },
                    { 7, "Nóc nhà Đông Dương, điểm đến lý tưởng cho người yêu thích chinh phục độ cao.", 800000.0, 1, "Đỉnh Fansipan" },
                    { 8, "Ngôi làng cổ đẹp như tranh của người H'Mông giữa núi rừng Tây Bắc.", 70000.0, 1, "Bản Cát Cát" },
                    { 9, "Di sản văn hóa và thiên nhiên thế giới, nổi tiếng với hệ thống hang động và sông nước.", 250000.0, 23, "Khu du lịch Tràng An" },
                    { 10, "Ngôi chùa lớn nhất Đông Nam Á, sở hữu nhiều kỷ lục châu Á và Việt Nam.", 0.0, 23, "Chùa Bái Đính" },
                    { 11, "Đường lên tiên cảnh với Cầu Vàng nổi tiếng thế giới và làng Pháp cổ kính.", 850000.0, 32, "Bà Nà Hills" },
                    { 12, "Được tạp chí Forbes bình chọn là một trong những bãi biển quyến rũ nhất hành tinh.", 0.0, 32, "Biển Mỹ Khê" },
                    { 13, "Quần thể 5 ngọn núi đá vôi với nhiều hang động và chùa chiền linh thiêng.", 40000.0, 32, "Ngũ Hành Sơn" },
                    { 14, "Di sản văn hóa thế giới, nơi lưu giữ vẻ đẹp cổ kính và yên bình bên dòng sông Hoài.", 0.0, 33, "Phố cổ Hội An" },
                    { 15, "Di tích lịch sử quan trọng, biểu tượng của sự thống nhất đất nước.", 40000.0, 45, "Dinh Độc Lập" },
                    { 16, "Biểu tượng văn hóa và thương mại lâu đời của Sài Gòn.", 0.0, 45, "Chợ Bến Thành" }
                });

            migrationBuilder.InsertData(
                table: "Medias",
                columns: new[] { "MediaId", "MediaUrl", "ScenicSpotId" },
                values: new object[,]
                {
                    { 1, "/uploads/hanoi/hoankiem.jpg", 1 },
                    { 2, "/uploads/hanoi/vanmieu.jpg", 2 },
                    { 3, "/uploads/quangninh/halong.jpg", 5 },
                    { 4, "/uploads/laocai/fansipan.jpg", 7 },
                    { 5, "/uploads/ninhbinh/trangan.jpg", 9 },
                    { 6, "/uploads/danang/banahills.jpg", 11 },
                    { 7, "/uploads/quangnam/hoian.jpg", 14 },
                    { 8, "/uploads/hcm/dinhdoclap.jpg", 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestionRequests_OrderId",
                table: "AISuggestionRequests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestionRequests_UserId",
                table: "AISuggestionRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AISuggestionRequests");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Medias",
                keyColumn: "MediaId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 15);
        }
    }
}
