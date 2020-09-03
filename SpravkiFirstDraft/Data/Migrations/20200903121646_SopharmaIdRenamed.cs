using Microsoft.EntityFrameworkCore.Migrations;

namespace SpravkiFirstDraft.Data.Migrations
{
    public partial class SopharmaIdRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sopharma",
                table: "Pharmacies");

            migrationBuilder.AddColumn<int>(
                name: "SopharmaId",
                table: "Pharmacies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SopharmaId",
                table: "Pharmacies");

            migrationBuilder.AddColumn<int>(
                name: "Sopharma",
                table: "Pharmacies",
                type: "int",
                nullable: true);
        }
    }
}
