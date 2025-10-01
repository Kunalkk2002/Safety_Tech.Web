using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safety_Tech.Models.Migrations
{
    /// <inheritdoc />
    public partial class dropObjectdetectiontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectdetection_AspNetUsers_ApproverId",
                table: "Objectdetection");

            migrationBuilder.DropTable(
                name: "ApprovedIncident");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objectdetection",
                table: "Objectdetection");

            migrationBuilder.RenameTable(
                name: "Objectdetection",
                newName: "Incidents");

            migrationBuilder.RenameIndex(
                name: "IX_Objectdetection_ApproverId",
                table: "Incidents",
                newName: "IX_Incidents_ApproverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Incidents",
                table: "Incidents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_AspNetUsers_ApproverId",
                table: "Incidents",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_AspNetUsers_ApproverId",
                table: "Incidents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Incidents",
                table: "Incidents");

            migrationBuilder.RenameTable(
                name: "Incidents",
                newName: "Objectdetection");

            migrationBuilder.RenameIndex(
                name: "IX_Incidents_ApproverId",
                table: "Objectdetection",
                newName: "IX_Objectdetection_ApproverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objectdetection",
                table: "Objectdetection",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ApprovedIncident",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproveBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    IsApprove = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedIncident", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovedIncident_AspNetUsers_ApproveBy",
                        column: x => x.ApproveBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovedIncident_Objectdetection_IncidentId",
                        column: x => x.IncidentId,
                        principalTable: "Objectdetection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedIncident_ApproveBy",
                table: "ApprovedIncident",
                column: "ApproveBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedIncident_IncidentId",
                table: "ApprovedIncident",
                column: "IncidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectdetection_AspNetUsers_ApproverId",
                table: "Objectdetection",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
