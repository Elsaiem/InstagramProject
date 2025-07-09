using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Comment;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Comment;
using InstagramProject.Core.ServiceContract;
using InstagramProject.Repository.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InstagramProject.Service.Comment
{
	public class CommentService : ICommentService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public CommentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<Result<CreateCommentResponse>> CreateCommentAsync(string userId, CreateCommentRequest request, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<CreateCommentResponse>(CommentErrors.UnauthorizedAccess);

			var post = await _context.posts.FirstOrDefaultAsync(p => p.Id == request.PostId, cancellationToken);
			if (post is null)
				return Result.Failure<CreateCommentResponse>(CommentErrors.PostNotFound);

			if (request.ParentCommentId is not null)
			{
				var parentComment = await _context.comments.FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value, cancellationToken);

				if (parentComment is null)
					return Result.Failure<CreateCommentResponse>(CommentErrors.ParentCommentNotFound);

				if (parentComment.PostId != request.PostId)
					return Result.Failure<CreateCommentResponse>(CommentErrors.ParentCommentNotInSamePost);
			}
			var comment = new Core.Entities.Comment
			{
				Content = request.Content,
				PostId = request.PostId,
				ParentCommentId = request.ParentCommentId,
				UserId = userId,
				Time = DateTime.UtcNow,
				IsReported = false
			};
			_context.comments.Add(comment);
			await _context.SaveChangesAsync(cancellationToken);

			var repliesCount = 0;
			if (!request.ParentCommentId.HasValue)
				repliesCount = await _context.comments.CountAsync(c => c.ParentCommentId == comment.Id, cancellationToken);

			var response = new CreateCommentResponse(
				CommentId: comment.Id,
				Content: comment.Content,
				PostId: comment.PostId,
				ParentCommentId: comment.ParentCommentId,
				UserId: userId,
				UserName: user.UserName!,
				Time: comment.Time,
				RepliesCount: repliesCount
			);
			return Result.Success(response);
		}
		public async Task<Result<PaginatedList<CommentDetailsResponse>>> GetCommentWithRepliesAsync(int postId, int commentId, RequestFilters filters, CancellationToken cancellationToken = default)
		{
			var postExists = await _context.posts.AnyAsync(p => p.Id == postId, cancellationToken);
			if (!postExists)
				return Result.Failure<PaginatedList<CommentDetailsResponse>>(CommentErrors.PostNotFound);

			var comment = await _context.comments.FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
			if (comment is null)
				return Result.Failure<PaginatedList<CommentDetailsResponse>>(CommentErrors.CommentNotFound);

			if (comment.PostId != postId)
				return Result.Failure<PaginatedList<CommentDetailsResponse>>(CommentErrors.CommentNotInPost);

			var repliesQuery = _context.comments
				.Include(r => r.User)
				.Include(r => r.Reactions)
				.Where(r => r.ParentCommentId == commentId)
				.OrderBy(r => r.Time)
				.Select(r => new CommentDetailsResponse(
					r.Id,
					r.Content,
					r.PostId,
					r.ParentCommentId,
					r.UserId,
					r.User.UserName!,
					r.User.ProfilePic ?? "https://res.cloudinary.com/dbpstijmp/image/upload/v1751892675/awgq5wbmds1147xvxnld.png",
					r.Time,
					r.Reactions.Count(react => react.IsReaction),
					false
				));
			var paginatedList = await PaginatedList<CommentDetailsResponse>.CreateAsync(
				repliesQuery, 
				filters.PageNumber, 
				filters.PageSize, 
				cancellationToken);

			return Result.Success(paginatedList);
		}
		public async Task<Result> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken = default)
		{
			var comment = await _context.comments
				.Include(c => c.Replies)
				.FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

			if (comment is null)
				return Result.Failure(CommentErrors.CommentNotFound);

			if (comment.UserId != userId)
				return Result.Failure(CommentErrors.UnauthorizedAccess);

			if (comment.Replies.Any())
				_context.comments.RemoveRange(comment.Replies);

			_context.comments.Remove(comment);
			await _context.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
	}
}
