using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addIsActiveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLocked",
                schema: "dbo",
                table: "UserDetails",
                newName: "IsActive");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Images",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Blogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "Advertisements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "dbo",
                table: "AdvertisementPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "dbo",
                table: "AdvertisementPackages");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "dbo",
                table: "UserDetails",
                newName: "IsLocked");
        }
    }
}
