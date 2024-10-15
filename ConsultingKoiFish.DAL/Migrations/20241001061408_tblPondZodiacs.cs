using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tblPondZodiacs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PondZodiacs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    ZodiacId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondZodiacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PondZodiacs_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PondZodiacs_Zodiacs_ZodiacId",
                        column: x => x.ZodiacId,
                        principalSchema: "dbo",
                        principalTable: "Zodiacs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PondZodiacs_PondId",
                schema: "dbo",
                table: "PondZodiacs",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondZodiacs_ZodiacId",
                schema: "dbo",
                table: "PondZodiacs",
                column: "ZodiacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PondZodiacs",
                schema: "dbo");
        }
    }
}
