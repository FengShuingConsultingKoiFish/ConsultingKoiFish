using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addBlogCommentAndAdCommentTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_ApplicationUser_UserId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AdvertisementId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_BlogId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AdvertisementId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "BlogId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.CreateTable(
                name: "AdComments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementId = table.Column<int>(type: "int", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdComments_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalSchema: "dbo",
                        principalTable: "Advertisements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlogComments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    BlogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogComments_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalSchema: "dbo",
                        principalTable: "Blogs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlogComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "dbo",
                        principalTable: "Comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdComments_AdvertisementId",
                schema: "dbo",
                table: "AdComments",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdComments_CommentId",
                schema: "dbo",
                table: "AdComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_BlogId",
                schema: "dbo",
                table: "BlogComments",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_CommentId",
                schema: "dbo",
                table: "BlogComments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Comments",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Payments",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId",
                principalSchema: "dbo",
                principalTable: "AdvertisementPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_ApplicationUser_UserId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedPackages_ApplicationUser_UserId",
                schema: "dbo",
                table: "PurchasedPackages");

            migrationBuilder.DropTable(
                name: "AdComments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BlogComments",
                schema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "AdvertisementId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                schema: "dbo",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AdvertisementId",
                schema: "dbo",
                table: "Comments",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BlogId",
                schema: "dbo",
                table: "Comments",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Advertisements_AdvertisementId",
                schema: "dbo",
                table: "Comments",
                column: "AdvertisementId",
                principalSchema: "dbo",
                principalTable: "Advertisements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Comments",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Blogs_BlogId",
                schema: "dbo",
                table: "Comments",
                column: "BlogId",
                principalSchema: "dbo",
                principalTable: "Blogs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ApplicationUser_UserId",
                schema: "dbo",
                table: "Payments",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_AdvertisementPackages_AdvertisementPackageId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "AdvertisementPackageId",
                principalSchema: "dbo",
                principalTable: "AdvertisementPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedPackages_ApplicationUser_UserId",
                schema: "dbo",
                table: "PurchasedPackages",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
