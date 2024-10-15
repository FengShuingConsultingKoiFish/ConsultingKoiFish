using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tblKoiBreedZodiacs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KoiBreedZodiacs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KoiBreedId = table.Column<int>(type: "int", nullable: false),
                    ZodiacId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiBreedZodiacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiBreedZodiacs_KoiBreeds_KoiBreedId",
                        column: x => x.KoiBreedId,
                        principalSchema: "dbo",
                        principalTable: "KoiBreeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KoiBreedZodiacs_Zodiacs_ZodiacId",
                        column: x => x.ZodiacId,
                        principalSchema: "dbo",
                        principalTable: "Zodiacs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KoiBreedZodiacs_KoiBreedId",
                schema: "dbo",
                table: "KoiBreedZodiacs",
                column: "KoiBreedId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiBreedZodiacs_ZodiacId",
                schema: "dbo",
                table: "KoiBreedZodiacs",
                column: "ZodiacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KoiBreedZodiacs",
                schema: "dbo");
        }
    }
}
