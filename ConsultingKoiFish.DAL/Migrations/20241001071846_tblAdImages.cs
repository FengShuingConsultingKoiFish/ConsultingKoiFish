using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tblAdImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdImages",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementId = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdImages_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalSchema: "dbo",
                        principalTable: "Advertisements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdImages_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "dbo",
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdImages_AdvertisementId",
                schema: "dbo",
                table: "AdImages",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdImages_ImageId",
                schema: "dbo",
                table: "AdImages",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdImages",
                schema: "dbo");
        }
    }
}
