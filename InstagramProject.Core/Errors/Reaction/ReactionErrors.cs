using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace InstagramProject.Core.Errors.Reaction
{
	public static class ReactionErrors
	{
		public static readonly Error UserNotFound = new("Reaction.UserNotFound", "User not found.", StatusCodes.Status404NotFound);
		public static readonly Error PostNotFound = new("Reaction.PostNotFound", "Post not found.", StatusCodes.Status404NotFound);
		public static readonly Error CommentNotFound = new("Reaction.CommentNotFound", "Comment not found.", StatusCodes.Status404NotFound);
		public static readonly Error CommentNotInPost = new("Reaction.CommentNotInPost", "The comment does not belong to the specified post.", StatusCodes.Status400BadRequest);
	}
}
