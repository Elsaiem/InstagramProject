using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddmoreEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_AspNetUsers_UserId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Comment_CommentId",
                table: "Reaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Reaction_Post_PostId",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Reaction",
                newName: "reactions");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "posts");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "comments");

            migrationBuilder.RenameIndex(
                name: "IX_Reaction_UserId",
                table: "reactions",
                newName: "IX_reactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reaction_PostId",
                table: "reactions",
                newName: "IX_reactions_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Reaction_CommentId",
                table: "reactions",
                newName: "IX_reactions_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_UserId",
                table: "posts",
                newName: "IX_posts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "comments",
                newName: "IX_comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_PostId",
                table: "comments",
                newName: "IX_comments_PostId");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnableFollowerAndFollowing",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnableNotificationFollowing",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnableNotificationNewRelease",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_reactions",
                table: "reactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_posts",
                table: "posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ActionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => new { x.UserId, x.FollowId });
                    table.ForeignKey(
                        name: "FK_UserFollows_AspNetUsers_FollowId",
                        column: x => x.FollowId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "userSavedPosts",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    SavedOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSavedPosts", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_userSavedPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userSavedPosts_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0a20e01f-e5ea-4ce0-afbc-ac24742c2732",
                columns: new[] { "IsEnableFollowerAndFollowing", "IsEnableNotificationFollowing", "IsEnableNotificationNewRelease" },
                values: new object[] { false, false, false });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_FollowId",
                table: "UserFollows",
                column: "FollowId");

            migrationBuilder.CreateIndex(
                name: "IX_userSavedPosts_PostId",
                table: "userSavedPosts",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_posts_PostId",
                table: "comments",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_AspNetUsers_UserId",
                table: "posts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reactions_AspNetUsers_UserId",
                table: "reactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reactions_comments_CommentId",
                table: "reactions",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reactions_posts_PostId",
                table: "reactions",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_posts_PostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_AspNetUsers_UserId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_reactions_AspNetUsers_UserId",
                table: "reactions");

            migrationBuilder.DropForeignKey(
                name: "FK_reactions_comments_CommentId",
                table: "reactions");

            migrationBuilder.DropForeignKey(
                name: "FK_reactions_posts_PostId",
                table: "reactions");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropTable(
                name: "userSavedPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reactions",
                table: "reactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_posts",
                table: "posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "IsEnableFollowerAndFollowing",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEnableNotificationFollowing",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEnableNotificationNewRelease",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "reactions",
                newName: "Reaction");

            migrationBuilder.RenameTable(
                name: "posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_reactions_UserId",
                table: "Reaction",
                newName: "IX_Reaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_reactions_PostId",
                table: "Reaction",
                newName: "IX_Reaction_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_reactions_CommentId",
                table: "Reaction",
                newName: "IX_Reaction_CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_UserId",
                table: "Post",
                newName: "IX_Post_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_PostId",
                table: "Comment",
                newName: "IX_Comment_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reaction",
                table: "Reaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Post_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_AspNetUsers_UserId",
                table: "Reaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Comment_CommentId",
                table: "Reaction",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reaction_Post_PostId",
                table: "Reaction",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
