using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class renameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                schema: "dbo",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                schema: "dbo",
                newName: "RefreshTokens",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                schema: "dbo",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "dbo",
                table: "RefreshTokens",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "dbo",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "dbo",
                newName: "RefreshToken",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "dbo",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                schema: "dbo",
                table: "RefreshToken",
                column: "Id");
        }
    }
}
