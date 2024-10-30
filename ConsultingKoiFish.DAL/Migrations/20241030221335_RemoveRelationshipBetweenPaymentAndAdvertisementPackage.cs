using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRelationshipBetweenPaymentAndAdvertisementPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages");

            // migrationBuilder.DropIndex(
            //     name: "IX_PurchasedPackages_AdvertisementPackageId",
            //     schema: "dbo",
            //     table: "PurchasedPackages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateIndex(
            //     name: "IX_PurchasedPackages_AdvertisementPackageId",
            //     schema: "dbo",
            //     table: "PurchasedPackages",
            //     column: "AdvertisementPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId",
                principalSchema: "dbo",
                principalTable: "AdvertisementPackages",
                principalColumn: "Id");
        }
    }
}
