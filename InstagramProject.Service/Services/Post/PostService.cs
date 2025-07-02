using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Post;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
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

			var postImages = new List<PostImages>();

			foreach (var postFile in request.PostImages)
			{
				var result = await _fileService.UploadToCloudinaryAsync(postFile);

				if (result.IsFailure)
					continue;

				postImages.Add(new PostImages(result.Value.SecureUrl));
			}
			if (postImages.Any())
			{
				var imageUrls = postImages.Select(img => img.PostImage).ToList();
				var imagesJson = JsonSerializer.Serialize(imageUrls);
				
				var response = new Core.Entities.Post
				{
					UserId = user.Id,
					Content = request.Content,
					Image = imagesJson,
					Time = DateTime.UtcNow
				};
				await _context.posts.AddAsync(response, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);
			}

			return Result.Success(new PostResponse(user.Id, request.Content, postImages));
		}
	}
}
