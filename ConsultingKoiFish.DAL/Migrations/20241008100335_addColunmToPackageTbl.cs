using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addColunmToPackageTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Limit",
                schema: "dbo",
                table: "AdvertisementPackages");

            migrationBuilder.AddColumn<int>(
                name: "LimitAd",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitContent",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitImage",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitAd",
                schema: "dbo",
                table: "AdvertisementPackages");

            migrationBuilder.DropColumn(
                name: "LimitContent",
                schema: "dbo",
                table: "AdvertisementPackages");

            migrationBuilder.DropColumn(
                name: "LimitImage",
                schema: "dbo",
                table: "AdvertisementPackages");

            migrationBuilder.AddColumn<int>(
                name: "Limit",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
