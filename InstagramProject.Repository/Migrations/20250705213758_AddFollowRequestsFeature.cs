using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowRequestsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEnableNotificationNewRelease",
                table: "AspNetUsers",
                newName: "IsEnablePublicOrPrivate");

            migrationBuilder.CreateTable(
                name: "FollowRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequesterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowRequests_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FollowRequests_AspNetUsers_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowRequests_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FollowRequests_ApplicationUserId",
                table: "FollowRequests",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowRequests_RequesterId",
                table: "FollowRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowRequests_TargetUserId",
                table: "FollowRequests",
                column: "TargetUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowRequests");

            migrationBuilder.RenameColumn(
                name: "IsEnablePublicOrPrivate",
                table: "AspNetUsers",
                newName: "IsEnableNotificationNewRelease");
        }
    }
}
