using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostTocontainsVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "posts",
                newName: "PostMedia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostMedia",
                table: "posts",
                newName: "Image");
        }
    }
}
