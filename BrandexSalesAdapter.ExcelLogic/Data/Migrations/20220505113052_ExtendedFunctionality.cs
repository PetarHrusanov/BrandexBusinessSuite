using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrandexSalesAdapter.ExcelLogic.Data.Migrations
{
    public partial class ExtendedFunctionality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Sales",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Sales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Sales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Regions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Regions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Regions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Regions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PharmacyChains",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "PharmacyChains",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PharmacyChains",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "PharmacyChains",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Pharmacies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Pharmacies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Pharmacies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Pharmacies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Distributors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Distributors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Distributors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Distributors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Cities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Cities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Cities",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PharmacyChains");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "PharmacyChains");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PharmacyChains");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "PharmacyChains");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Distributors");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Cities");
        }
    }
}
