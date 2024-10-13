using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addKoiDetailTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondDetails_KoiBreeds_KoiBreedId",
                schema: "dbo",
                table: "PondDetails");

            migrationBuilder.DropIndex(
                name: "IX_PondDetails_KoiBreedId",
                schema: "dbo",
                table: "PondDetails");

            migrationBuilder.DropColumn(
                name: "KoiBreedId",
                schema: "dbo",
                table: "PondDetails");

            migrationBuilder.CreateTable(
                name: "KoiDetails",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserPondId = table.Column<int>(type: "int", nullable: false),
                    KoiBreedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KoiDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KoiDetails_KoiBreeds_KoiBreedId",
                        column: x => x.KoiBreedId,
                        principalSchema: "dbo",
                        principalTable: "KoiBreeds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KoiDetails_UserPonds_UserPondId",
                        column: x => x.UserPondId,
                        principalSchema: "dbo",
                        principalTable: "UserPonds",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KoiDetails_KoiBreedId",
                schema: "dbo",
                table: "KoiDetails",
                column: "KoiBreedId");

            migrationBuilder.CreateIndex(
                name: "IX_KoiDetails_UserPondId",
                schema: "dbo",
                table: "KoiDetails",
                column: "UserPondId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KoiDetails",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "KoiBreedId",
                schema: "dbo",
                table: "PondDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PondDetails_KoiBreedId",
                schema: "dbo",
                table: "PondDetails",
                column: "KoiBreedId");

            migrationBuilder.AddForeignKey(
                name: "FK_PondDetails_KoiBreeds_KoiBreedId",
                schema: "dbo",
                table: "PondDetails",
                column: "KoiBreedId",
                principalSchema: "dbo",
                principalTable: "KoiBreeds",
                principalColumn: "Id");
        }
    }
}
