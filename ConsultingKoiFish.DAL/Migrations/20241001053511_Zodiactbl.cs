using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Zodiactbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Role",
                keyColumn: "Id",
                keyValue: "3348dc22-3a71-4507-b18d-e22feb350bdd");

            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Role",
                keyColumn: "Id",
                keyValue: "b227598e-b882-45aa-8ca9-e6eb910d8b76");

            migrationBuilder.CreateTable(
                name: "Zodiacs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZodiacName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zodiacs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserZodiacs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ZodiacId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserZodiacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserZodiacs_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserZodiacs_Zodiacs_ZodiacId",
                        column: x => x.ZodiacId,
                        principalSchema: "dbo",
                        principalTable: "Zodiacs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserZodiacs_UserId",
                schema: "dbo",
                table: "UserZodiacs",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserZodiacs_ZodiacId",
                schema: "dbo",
                table: "UserZodiacs",
                column: "ZodiacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserZodiacs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Zodiacs",
                schema: "dbo");

            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3348dc22-3a71-4507-b18d-e22feb350bdd", "2", "Member", "MEMBER" },
                    { "b227598e-b882-45aa-8ca9-e6eb910d8b76", "1", "Admin", "ADMIN" }
                });
        }
    }
}
