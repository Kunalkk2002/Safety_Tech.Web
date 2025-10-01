using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safety_Tech.Models.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproveBy",
                table: "Objectdetection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApproverId",
                table: "Objectdetection",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                table: "Objectdetection",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Objectdetection_ApproverId",
                table: "Objectdetection",
                column: "ApproverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectdetection_AspNetUsers_ApproverId",
                table: "Objectdetection",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectdetection_AspNetUsers_ApproverId",
                table: "Objectdetection");

            migrationBuilder.DropIndex(
                name: "IX_Objectdetection_ApproverId",
                table: "Objectdetection");

            migrationBuilder.DropColumn(
                name: "ApproveBy",
                table: "Objectdetection");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Objectdetection");

            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "Objectdetection");
        }
    }
}
