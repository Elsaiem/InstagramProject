using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace InstagramProject.Core.Errors.Post
{
	public static class PostErrors
	{
		public static readonly Error PostNotFound = new("Post.PostNotFound", "Post Not Found", StatusCodes.Status404NotFound);
        public static readonly Error ImageUploadFailed = new("Post.ImageUploadFailed", "Failed to upload post image", StatusCodes.Status400BadRequest);
        public static readonly Error MediaDeletionFailed = new("Post.MediaDeletionFailed", "Failed to delete media from storage", StatusCodes.Status400BadRequest);
        public static readonly Error UnauthorizedAccess = new("Post.UnauthorizedAccess", "You not authorized to perform this action", StatusCodes.Status403Forbidden);
        public static readonly Error PostDeletionFailed = new("Post.PostDeletionFailed", "Failed to delete post", StatusCodes.Status400BadRequest);
	}
}
