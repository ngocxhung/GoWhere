using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class updatecustomerscheduletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CustomerSchedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerSchedules");
        }
    }
}
