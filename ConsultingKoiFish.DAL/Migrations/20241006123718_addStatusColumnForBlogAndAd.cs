using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addStatusColumnForBlogAndAd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MornitoredQuantity",
                schema: "dbo",
                table: "PurchasedPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "dbo",
                table: "Advertisements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements",
                column: "PurchasedPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisements_PurchasedPackages_PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements",
                column: "PurchasedPackageId",
                principalSchema: "dbo",
                principalTable: "PurchasedPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisements_PurchasedPackages_PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Advertisements_PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "MornitoredQuantity",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "PurchasedPackageId",
                schema: "dbo",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "dbo",
                table: "Advertisements");
        }
    }
}
