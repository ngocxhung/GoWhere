using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsViewedToScenicSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestionRequests_Order_OrderId",
                table: "AISuggestionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Users_UserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "OrderItem",
                newName: "OrderItems");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_UserId",
                table: "Orders",
                newName: "IX_Orders_UserId");

            migrationBuilder.AddColumn<bool>(
                name: "IsViewed",
                table: "ScenicSpots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "OrderItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "OrderId");

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 1,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 2,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 3,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 4,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 5,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 6,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 7,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 8,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 9,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 10,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 11,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 12,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 13,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 14,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 15,
                column: "IsViewed",
                value: true);

            migrationBuilder.UpdateData(
                table: "ScenicSpots",
                keyColumn: "ScenicSpotId",
                keyValue: 16,
                column: "IsViewed",
                value: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestionRequests_Orders_OrderId",
                table: "AISuggestionRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestionRequests_Orders_OrderId",
                table: "AISuggestionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "IsViewed",
                table: "ScenicSpots");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItem");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserId",
                table: "Order",
                newName: "IX_Order_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItem",
                table: "OrderItem",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestionRequests_Order_OrderId",
                table: "AISuggestionRequests",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Users_UserId",
                table: "Order",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_OrderId",
                table: "OrderItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
