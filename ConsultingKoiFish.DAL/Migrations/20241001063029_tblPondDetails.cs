using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tblPondDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PondDetails",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondId = table.Column<int>(type: "int", nullable: false),
                    KoiBreedId = table.Column<int>(type: "int", nullable: false),
                    UserPondId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PondDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PondDetails_KoiBreeds_KoiBreedId",
                        column: x => x.KoiBreedId,
                        principalSchema: "dbo",
                        principalTable: "KoiBreeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PondDetails_Ponds_PondId",
                        column: x => x.PondId,
                        principalSchema: "dbo",
                        principalTable: "Ponds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PondDetails_UserPonds_UserPondId",
                        column: x => x.UserPondId,
                        principalSchema: "dbo",
                        principalTable: "UserPonds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PondDetails_KoiBreedId",
                schema: "dbo",
                table: "PondDetails",
                column: "KoiBreedId");

            migrationBuilder.CreateIndex(
                name: "IX_PondDetails_PondId",
                schema: "dbo",
                table: "PondDetails",
                column: "PondId");

            migrationBuilder.CreateIndex(
                name: "IX_PondDetails_UserPondId",
                schema: "dbo",
                table: "PondDetails",
                column: "UserPondId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PondDetails",
                schema: "dbo");
        }
    }
}
