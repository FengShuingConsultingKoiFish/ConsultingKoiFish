using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class tblImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondDetails_UserPonds_UserPondId",
                schema: "dbo",
                table: "PondDetails");

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ApplicationUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_UserId",
                schema: "dbo",
                table: "Images",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PondDetails_UserPonds_UserPondId",
                schema: "dbo",
                table: "PondDetails",
                column: "UserPondId",
                principalSchema: "dbo",
                principalTable: "UserPonds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PondDetails_UserPonds_UserPondId",
                schema: "dbo",
                table: "PondDetails");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "dbo");

            migrationBuilder.AddForeignKey(
                name: "FK_PondDetails_UserPonds_UserPondId",
                schema: "dbo",
                table: "PondDetails",
                column: "UserPondId",
                principalSchema: "dbo",
                principalTable: "UserPonds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
