using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Contracts.Files;
using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Post;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
using InstagramProject.Service.Services.Files;
using Microsoft.AspNetCore.Identity;
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

			var response = new Core.Entities.Post
			{
				UserId = user.Id,
				Content = request.Content,
				PostMedia = mediaJson,
				Time = DateTime.UtcNow
			};
			await _context.posts.AddAsync(response, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success(new PostResponse(user.Id, request.Content, postMedia));
		}
	}
}
