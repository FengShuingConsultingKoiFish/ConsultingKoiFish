using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class updatDbContextForPaymentFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationsInDays",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LimitAd",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitContent",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitImage",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ClonePackage",
                schema: "dbo",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationsInDays",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "DurationsInDays",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "LimitAd",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "LimitContent",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "LimitImage",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "ClonePackage",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DurationsInDays",
                schema: "dbo",
                table: "AdvertisementPackages");
        }
    }
}
