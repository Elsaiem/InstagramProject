using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Editonfollowrequestconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_AspNetUsers_RequesterId",
                table: "FollowRequests",
                column: "RequesterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
