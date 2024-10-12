using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class resetRelationshipPurchasedAndPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchasedPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PurchasedPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasedPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId");
        }
    }
}
