using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace InstagramProject.Core.Errors.Post
{
	public static class PostErrors
	{
		public static readonly Error PostNotFound = new("Post.PostNotFound", "Post Not Found", StatusCodes.Status404NotFound);
		public static readonly Error UserNotFound = new("Post.UserNotFound", "User Not Found", StatusCodes.Status404NotFound);
        public static readonly Error ImageUploadFailed = new("Post.ImageUploadFailed", "Failed to upload post image", StatusCodes.Status400BadRequest);
	}
}
