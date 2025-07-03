using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Authentication;
using InstagramProject.Core.Abstractions;
using InstagramProject.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InstagramProject.Core.Service_contract;

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
		public async Task<Result<IEnumerable<FeedResponse>>> UserFeedAsync(string userId, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<IEnumerable<FeedResponse>>(UserErrors.UserNotFound);

			var following = await _context.UserFollows
				.Where(uf => uf.UserId == userId)
				.Select(uf => uf.FollowId)
				.ToListAsync(cancellationToken);
			
			if (!following.Any())
				return Result.Success(Enumerable.Empty<FeedResponse>());

			var feedPosts = await _context.posts
				.Include(p => p.User)
				.Include(p => p.Reactions)
				.Include(p => p.Comments)
				.Where(p => following.Contains(p.UserId))
				.OrderByDescending(p => p.Time)
				.ToListAsync(cancellationToken);

			var feedResponses = feedPosts
				.GroupBy(p => p.UserId)
				.Select(group => 
				{
					var firstPost = group.First();
					return new FeedResponse(
						PostId: firstPost.Id,
						userId: firstPost.UserId,
						userName: firstPost.User.UserName,
						Time: firstPost.Time,
						posts: group.Select(p => new FeedPostResponse(p.PostMedia ?? string.Empty)),
						Likes: group.Sum(p => p.Reactions.Count(r => r.IsReaction)),
						Comments: group.Sum(p => p.Comments.Count)
					);
				})
				.ToList();

			return Result.Success<IEnumerable<FeedResponse>>(feedResponses);
		}
		public async Task<Result<IEnumerable<SearchResponse>>> SearchForUserAdync(SearchRequest request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.SearchValue))
				return Result.Success<IEnumerable<SearchResponse>>(new List<SearchResponse>());

			var searchTerm = request.SearchValue.Trim().ToLower();
			var normalizedSearchTerm = searchTerm.Replace(" ", "");

			var userResults = await _context.Users
				.Where(u => !u.IsDisabled &&
					(u.UserName.ToLower().Contains(searchTerm) || u.UserName.ToLower().Replace(" ", "").Contains(normalizedSearchTerm)))
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
	}
}
