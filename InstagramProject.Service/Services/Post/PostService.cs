using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Comment;
using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Post;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
using InstagramProject.Service.Services.Files;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InstagramProject.Service.Services.Post
{
	public class PostService : IPostService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IFileService _fileService;

		public PostService(
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			IFileService fileService)
		{
			_context = context;
			_userManager = userManager;
			_fileService = fileService;
		}
		public async Task<Result<PostResponse>> CreatePostAsunc(CreatePostRequest request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.userId);
			if (user is null)
				return Result.Failure<PostResponse>(PostErrors.UserNotFound);

			var postMedia = new List<PostMedia>();
			foreach (var mediaFile in request.PostMedia)
			{
				var result = await _fileService.UploadToCloudinaryAsync(mediaFile);

				if (result.IsSuccess)
				{
					var mediaType = CloudinaryService.IsVideoFile(mediaFile) ? "video" : "image";
					postMedia.Add(new PostMedia(result.Value.SecureUrl, mediaType));
				}
			}
			var mediaUrls = postMedia.Select(media => new { url = media.MediaUrl, type = media.MediaType }).ToList();
			var mediaJson = JsonSerializer.Serialize(mediaUrls);

			var post = new Core.Entities.Post
			{
				UserId = user.Id,
				Content = request.Content,
				PostMedia = mediaJson,
				Time = DateTime.UtcNow
			};
			await _context.posts.AddAsync(post, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success(new PostResponse(post.Id, user.Id, request.Content, postMedia));
		}
		public async Task<Result<PostDetailsResponse>> GetPostAsync(int postId, CancellationToken cancellationToken)
		{
			var post = await _context.posts
				.Include(p => p.User)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User)
				.AsNoTracking()
				.FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

			if (post == null)
				return Result.Failure<PostDetailsResponse>(PostErrors.PostNotFound);

			var media = new List<PostMedia>();
			if (!string.IsNullOrEmpty(post.PostMedia))
			{
				var mediaObjects = JsonSerializer.Deserialize<List<dynamic>>(post.PostMedia);
				if (mediaObjects != null)
				{
					foreach (var mediaObj in mediaObjects)
					{
						var mediaElement = mediaObj;
						var url = mediaElement.GetProperty("url").GetString();
						var type = mediaElement.GetProperty("type").GetString();

						if (url != null && type != null)
							media.Add(new PostMedia(url, type));
					}
				}
			}
			var comments = post.Comments.Adapt<List<CommentPostResponse>>();
			var response = new PostDetailsResponse(
				post.Id,
				post.UserId,
				post.User.UserName!,
				post.Time,
				post.Content,
				media,
				comments
			);
			return Result.Success(response);
		}
		public async Task<Result<PostResponse>> UpdatePostAsync(UpdatePostRequest request, CancellationToken cancellationToken)
		{
			var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
			if (post == null)
				return Result.Failure<PostResponse>(PostErrors.PostNotFound);

			if (post.UserId != request.UserId)
				return Result.Failure<PostResponse>(PostErrors.UserNotFound);

			if (!string.IsNullOrEmpty(request.Content))
				post.Content = request.Content;

			if (request.Media != null && request.Media.Any())
			{
				var media = new List<PostMedia>();
				if (!string.IsNullOrEmpty(post.PostMedia))
				{
					var mediaObjects = JsonSerializer.Deserialize<List<JsonElement>>(post.PostMedia);
					if (mediaObjects != null)
					{
						foreach (var mediaObj in mediaObjects)
						{
							var mediaElement = mediaObj;
							var url = mediaElement.GetProperty("url").GetString();
							var type = mediaElement.GetProperty("type").GetString();

							if (url != null && type != null)
								media.Add(new PostMedia(url, type));
						}
					}
				}
				var removeMedia = media.Where(m => request.Media.Contains(m.MediaUrl)).ToList();
				foreach (var item in removeMedia)
				{
					var publicId = CloudinaryService.ExtractPublicIdFromUrl(item.MediaUrl);
					if (!string.IsNullOrEmpty(publicId))
					{
						var deleteResult = await _fileService.DeleteFromCloudinaryAsync(publicId);
						if (!deleteResult.IsSuccess)
						{
							return Result.Failure<PostResponse>(PostErrors.MediaDeletionFailed);
						}
					}
				}
				var finalMedia = media.Where(m => !request.Media.Contains(m.MediaUrl)).ToList();
				if (finalMedia.Count == 0)
				{
					_context.posts.Remove(post);
					await _context.SaveChangesAsync(cancellationToken);
					return Result.Success(new PostResponse(post.Id, post.UserId, post.Content, new List<PostMedia>()));
				}
				
				var allMedia = finalMedia.Select(m => new { url = m.MediaUrl, type = m.MediaType }).ToList();
				post.PostMedia = JsonSerializer.Serialize(allMedia);
			}
			await _context.SaveChangesAsync(cancellationToken);
			var mediaResponse = new List<PostMedia>();
			if (!string.IsNullOrEmpty(post.PostMedia))
			{
				var mediaObjects = JsonSerializer.Deserialize<List<JsonElement>>(post.PostMedia);
				if (mediaObjects != null)
				{
					foreach (var mediaObj in mediaObjects)
					{
						var mediaElement = mediaObj;
						var url = mediaElement.GetProperty("url").GetString();
						var type = mediaElement.GetProperty("type").GetString();

						if (url != null && type != null)
							mediaResponse.Add(new PostMedia(url, type));
					}
				}
			}
			return Result.Success(new PostResponse(post.Id, post.UserId, post.Content, mediaResponse));
		}
		public async Task<Result> DeletePostAsync(DeletePostRequest request, CancellationToken cancellationToken)
		{
			var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
			if (post == null)
				return Result.Failure(PostErrors.PostNotFound);

			if (post.UserId != request.UserId)
				return Result.Failure(PostErrors.UnauthorizedAccess);

			if (!string.IsNullOrEmpty(post.PostMedia))
			{
				var mediaObjects = JsonSerializer.Deserialize<List<JsonElement>>(post.PostMedia);
				if (mediaObjects != null)
				{
					foreach (var mediaObj in mediaObjects)
					{
						var mediaElement = mediaObj;
						var url = mediaElement.GetProperty("url").GetString();

						if (url != null)
						{
							var publicId = CloudinaryService.ExtractPublicIdFromUrl(url);
							if (!string.IsNullOrEmpty(publicId))
							{
								var deleteResult = await _fileService.DeleteFromCloudinaryAsync(publicId);
								if (!deleteResult.IsSuccess)
								{
									return Result.Failure(PostErrors.MediaDeletionFailed);
								}
							}
						}
					}
				}
			}
			_context.posts.Remove(post);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}
	}
}
