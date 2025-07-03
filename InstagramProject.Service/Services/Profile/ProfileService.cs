using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Entities;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Authentication;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InstagramProject.Service.Services.Profile
{
	public class ProfileService : IProfileService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		public ProfileService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<Result> AddUserFollowAsync(string userId, string follwerId, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure(UserErrors.UserNotFound);

			var follower = await _userManager.FindByIdAsync(follwerId);
			if (follower is null)
				return Result.Failure(UserErrors.UserNotFound);

			if (userId == follwerId)
				return Result.Failure(UserErrors.CannotFollowYourself);

			var isRelationExsit = await _context.UserFollows.AnyAsync(uf => uf.UserId == userId && uf.FollowId == follwerId, cancellationToken);
			if (isRelationExsit)
				return Result.Failure(UserErrors.AlreadyFollowing);

			var userFollow = new UserFollow
			{
				UserId = userId,
				FollowId = follwerId,
				FollowedOn = DateTime.UtcNow
			};
			await _context.UserFollows.AddAsync(userFollow, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}
		public async Task<Result> DeleteUserFollowAsync(string userId, string follwerId, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure(UserErrors.UserNotFound);

			var follower = await _userManager.FindByIdAsync(follwerId);
			if (follower is null)
				return Result.Failure(UserErrors.UserNotFound);

			var isRelationExsit = await _context.UserFollows.AnyAsync(uf => uf.UserId == userId && uf.FollowId == follwerId, cancellationToken);
			if (!isRelationExsit)
				return Result.Failure(UserErrors.NotFollowing);

			_context.UserFollows.Remove(new UserFollow
			{
				UserId = userId,
				FollowId = follwerId
			});
			await _context.SaveChangesAsync(cancellationToken);
			return Result.Success();
		}

	}
}
