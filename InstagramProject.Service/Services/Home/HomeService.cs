using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Authentication;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InstagramProject.Service.Services.Home
{
	public class HomeService : IHomeService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public HomeService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<PaginatedList<FeedResponse>> UserFeedAsync(string userId, RequestFilters request, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return PaginatedList<FeedResponse>.Empty();

			var feed = _context.posts
				.Include(p => p.User)
				.Include(p => p.Reactions)
				.Include(p => p.Comments)
				.Where(p => _context.UserFollows
					.Where(uf => uf.UserId == userId)
					.Select(uf => uf.FollowId)
					.Contains(p.UserId))
				.OrderBy(p => p.Time)
				.AsNoTracking()
				.Select(p => new FeedResponse(
					p.Id,
					p.UserId,
					p.User.UserName ?? string.Empty,
					p.Time,
					string.IsNullOrEmpty(p.PostMedia)
						? new List<FeedPostResponse>()
						: ParsePostMedia(p.PostMedia),
					p.Reactions.Count(r => r.IsReaction),
					p.Comments.Count()
				));
			return await PaginatedList<FeedResponse>.CreateAsync(feed, request.PageNumber, request.PageSize, cancellationToken);
		}
		public async Task<Result<IEnumerable<SearchResponse>>> SearchForUserAdync(SearchRequest request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.SearchValue))
				return Result.Success<IEnumerable<SearchResponse>>(new List<SearchResponse>());

			var searchTerm = request.SearchValue.Trim().ToLower();
			var normalizedSearchTerm = searchTerm.Replace(" ", "");
			var userResults = await _context.Users
				.Where(u => !u.IsDisabled &&
					(u.UserName != null && (u.UserName.ToLower().Contains(searchTerm) || u.UserName.ToLower().Replace(" ", "").Contains(normalizedSearchTerm))))
				.Take(50)
				.Select(u => new SearchResponse(
					u.Id,
					u.UserName!,
					u.ProfilePic ?? string.Empty
				))
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return Result.Success<IEnumerable<SearchResponse>>(userResults);
		}
		private static List<FeedPostResponse> ParsePostMedia(string postMediaJson)
		{
			using var document = System.Text.Json.JsonDocument.Parse(postMediaJson);
			var mediaList = new List<FeedPostResponse>();

			foreach (var element in document.RootElement.EnumerateArray())
			{
				if (element.TryGetProperty("url", out var urlProperty))
				{
					var url = urlProperty.GetString() ?? string.Empty;
					mediaList.Add(new FeedPostResponse(url));
				}
			}
			return mediaList;
		}
	}
}
