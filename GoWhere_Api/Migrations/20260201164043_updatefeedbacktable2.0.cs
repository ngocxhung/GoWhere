using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoWhere_Api.Migrations
{
    /// <inheritdoc />
    public partial class updatefeedbacktable20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSchedules_FeedbackId",
                table: "CustomerSchedules");

            migrationBuilder.DropColumn(
                name: "CustomerScheduleId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Feedbacks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Feedbacks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSchedules_FeedbackId",
                table: "CustomerSchedules",
                column: "FeedbackId",
                unique: true,
                filter: "[FeedbackId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerSchedules_FeedbackId",
                table: "CustomerSchedules");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Feedbacks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerScheduleId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSchedules_FeedbackId",
                table: "CustomerSchedules",
                column: "FeedbackId");
        }
    }
}
