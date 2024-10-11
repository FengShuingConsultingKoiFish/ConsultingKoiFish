using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class setClientSetNullMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments",
                column: "AdvertisementId",
                principalSchema: "dbo",
                principalTable: "Advertisements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments",
                column: "BlogId",
                principalSchema: "dbo",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "BlogId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AdvertisementId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments",
                column: "AdvertisementId",
                principalSchema: "dbo",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments",
                column: "BlogId",
                principalSchema: "dbo",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
