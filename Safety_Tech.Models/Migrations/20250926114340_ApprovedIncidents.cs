using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safety_Tech.Models.Migrations
{
    /// <inheritdoc />
    public partial class ApprovedIncidents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovedIncident",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IncidentId = table.Column<int>(type: "int", nullable: false),
                    ApproveBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovedIncident");
        }
    }
}
