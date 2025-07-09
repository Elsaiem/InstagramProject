using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace InstagramProject.Core.Errors.Comment
{
	public static class CommentErrors
	{
		public static readonly Error CommentNotFound = new("Comment.CommentNotFound", "Comment not found", StatusCodes.Status404NotFound);
		public static readonly Error PostNotFound = new("Comment.PostNotFound", "Post not found", StatusCodes.Status404NotFound);
		public static readonly Error ParentCommentNotFound = new("Comment.ParentCommentNotFound", "Parent comment not found", StatusCodes.Status404NotFound);
		public static readonly Error ParentCommentNotInSamePost = new("Comment.ParentCommentNotInSamePost", "Parent comment does not belong to the same post", StatusCodes.Status400BadRequest);
		public static readonly Error CommentNotInPost = new("Comment.CommentNotInPost", "Comment does not belong to the specified post", StatusCodes.Status400BadRequest);
		public static readonly Error UnauthorizedAccess = new("Comment.UnauthorizedAccess", "You are not authorized to perform this action", StatusCodes.Status403Forbidden);
		public static readonly Error CommentCreationFailed = new("Comment.CommentCreationFailed", "Failed to create comment", StatusCodes.Status400BadRequest);
	}
}
