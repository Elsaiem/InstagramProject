using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Contracts.Reaction;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Reaction;
using InstagramProject.Core.ServiceContract;
using InstagramProject.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InstagramProject.Service.Reaction
{
	public class ReactionService : IReactionService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ReactionService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<Result<CreateReactionResponse>> CreateReactionAsync(string userId, CreateReactionRequest request, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<CreateReactionResponse>(ReactionErrors.UserNotFound);

			if (!await _context.posts.AnyAsync(p => p.Id == request.PostId, cancellationToken))
				return Result.Failure<CreateReactionResponse>(ReactionErrors.PostNotFound);

			if (request.CommentId.HasValue)
			{
				var comment = await _context.comments.FirstOrDefaultAsync(c => c.Id == request.CommentId.Value, cancellationToken);
				if (comment is null)
					return Result.Failure<CreateReactionResponse>(ReactionErrors.CommentNotFound);
				if (comment.PostId != request.PostId)
					return Result.Failure<CreateReactionResponse>(ReactionErrors.CommentNotInPost);
			}
			var existingReaction = await _context.reactions.FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == request.PostId && r.CommentId == request.CommentId,cancellationToken);
			Core.Entities.Reaction reaction;
			if (existingReaction != null)
			{
				reaction = existingReaction;
				_context.reactions.Remove(reaction);
			}
			else
			{
				reaction = new Core.Entities.Reaction
				{
					UserId = userId,
					PostId = request.PostId,
					CommentId = request.CommentId,
					IsReaction = true,
					Time = DateTime.UtcNow
				};
				_context.reactions.Add(reaction);
			}
			await _context.SaveChangesAsync(cancellationToken);
			var response = new CreateReactionResponse(
				ReactionId: reaction.Id,
				UserId: userId,
				UserName: user.UserName!,
				PostId: reaction.PostId,
				CommentId: reaction.CommentId,
				Time: reaction.Time
			);
			return Result.Success(response);
		}
	}
}
