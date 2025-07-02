using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIsReportedInPostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "posts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
