using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addPackageImageTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageImages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementPackageId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageImages_AdvertisementPackages_AdvertisementPackageId",
                        column: x => x.AdvertisementPackageId,
                        principalSchema: "dbo",
                        principalTable: "AdvertisementPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackageImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "dbo",
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageImages_AdvertisementPackageId",
                schema: "dbo",
                table: "PackageImages",
                column: "AdvertisementPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageImages_ImageId",
                schema: "dbo",
                table: "PackageImages",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageImages",
                schema: "dbo");
        }
    }
}
