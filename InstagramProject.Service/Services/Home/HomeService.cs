using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Contracts.Profile;
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
						? Enumerable.Empty<string>()
						: ParsePostMediaUrls(p.PostMedia),
					p.Reactions.Count(r => r.IsReaction),
					p.Comments.Count()
				));
			return await PaginatedList<FeedResponse>.CreateAsync(feed, request.PageNumber, request.PageSize, cancellationToken);
		}
		public async Task<Result<IEnumerable<SearchResponse>>> SearchForUserAsync(SearchRequest request, CancellationToken cancellationToken)
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
					u.FullName,
					u.ProfilePic ?? "https://res.cloudinary.com/dbpstijmp/image/upload/v1751892675/awgq5wbmds1147xvxnld.png"
				))
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			return Result.Success<IEnumerable<SearchResponse>>(userResults);
		}
		public async Task<Result<IEnumerable<SuggestionsFollowerResponse>>> SuggestionsFollowerForYouAsync(string userId, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null || user.IsDisabled)
				return Result.Failure<IEnumerable<SuggestionsFollowerResponse>>(UserErrors.UserNotFound);

			var followersId = await _context.UserFollows
				.Where(uf => uf.UserId == userId)
				.Select(uf => uf.FollowId)
				.ToListAsync(cancellationToken);

			var suggestedUsers = await _context.UserFollows
				.Where(uf => followersId.Contains(uf.UserId) && uf.FollowId != userId && !followersId.Contains(uf.FollowId))
				.GroupBy(uf => uf.FollowId)
				.Select(g => new { UserId = g.Key, MutualFriendsCount = g.Count() })
				.OrderByDescending(x => x.MutualFriendsCount)
				.Take(10)
				.ToListAsync(cancellationToken);

			var suggestedUserIds = suggestedUsers.Select(su => su.UserId).ToList();
			var userDetails = await _context.Users
				.Where(u => suggestedUserIds.Contains(u.Id))
				.Select(u => new { u.Id, u.UserName, u.ProfilePic })
				.ToListAsync(cancellationToken);

			var suggestions = suggestedUsers
				.Join(userDetails, su => su.UserId, ud => ud.Id, (su, ud) => new SuggestionsFollowerResponse(
					ud.UserName!,
					ud.ProfilePic ?? "https://res.cloudinary.com/dbpstijmp/image/upload/v1751892675/awgq5wbmds1147xvxnld.png",
					su.MutualFriendsCount
				)).ToList();

			return Result.Success<IEnumerable<SuggestionsFollowerResponse>>(suggestions);
		}
		private static IEnumerable<string> ParsePostMediaUrls(string postMediaJson)
		{
			using var document = System.Text.Json.JsonDocument.Parse(postMediaJson);
			var mediaUrls = new List<string>();

			foreach (var element in document.RootElement.EnumerateArray())
			{
				if (element.TryGetProperty("url", out var urlProperty))
				{
					var url = urlProperty.GetString() ?? string.Empty;
					if (!string.IsNullOrEmpty(url))
						mediaUrls.Add(url);
				}
			}
			return mediaUrls;
		}
	}
}
