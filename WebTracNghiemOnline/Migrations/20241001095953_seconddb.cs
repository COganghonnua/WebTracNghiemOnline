using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTracNghiemOnline.Migrations
{
    /// <inheritdoc />
    public partial class seconddb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "OnlineRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OnlineRooms_ExamId",
                table: "OnlineRooms",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnlineRooms_Exams_ExamId",
                table: "OnlineRooms",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnlineRooms_Exams_ExamId",
                table: "OnlineRooms");

            migrationBuilder.DropIndex(
                name: "IX_OnlineRooms_ExamId",
                table: "OnlineRooms");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "OnlineRooms");
        }
    }
}
