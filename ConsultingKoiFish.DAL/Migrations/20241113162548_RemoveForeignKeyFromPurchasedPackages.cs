using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveForeignKeyFromPurchasedPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages",
                table: "PurchasedPackages"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId",
                principalTable: "AdvertisementPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
