using Microsoft.EntityFrameworkCore.Migrations;

namespace SpravkiFirstDraft.Data.Migrations
{
    public partial class ProductsImproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "BrandexId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PharmnetId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoenixId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SopharmaId",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StingId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandexId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PharmnetId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PhoenixId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SopharmaId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StingId",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "DiscountPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
