using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultingKoiFish.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addRelationShipForComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogComments_CommentId",
                schema: "dbo",
                table: "BlogComments");

            migrationBuilder.DropIndex(
                name: "IX_AdComments_CommentId",
                schema: "dbo",
                table: "AdComments");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_CommentId",
                schema: "dbo",
                table: "BlogComments",
                column: "CommentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdComments_CommentId",
                schema: "dbo",
                table: "AdComments",
                column: "CommentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogComments_CommentId",
                schema: "dbo",
                table: "BlogComments");

            migrationBuilder.DropIndex(
                name: "IX_AdComments_CommentId",
                schema: "dbo",
                table: "AdComments");

            migrationBuilder.CreateIndex(
                name: "IX_BlogComments_CommentId",
                schema: "dbo",
                table: "BlogComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AdComments_CommentId",
                schema: "dbo",
                table: "AdComments",
                column: "CommentId");
        }
    }
}
