using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddScenicSpotToOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScenicSpotId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ScenicSpotId",
                table: "OrderItems",
                column: "ScenicSpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ScenicSpots_ScenicSpotId",
                table: "OrderItems",
                column: "ScenicSpotId",
                principalTable: "ScenicSpots",
                principalColumn: "ScenicSpotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ScenicSpots_ScenicSpotId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ScenicSpotId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ScenicSpotId",
                table: "OrderItems");
        }
    }
}
