using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Comment;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Extensions;
using InstagramProject.Core.ServiceContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CommentController : ControllerBase
	{
		private readonly ICommentService _commentService;
		public CommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}
		[HttpPost]
		public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
		{
			var response = await _commentService.CreateCommentAsync(User.GetUserId()!, request, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
		[HttpGet("{postId}/comment/{commentId}")]
		public async Task<IActionResult> GetCommentWithReplies([FromRoute] int postId, [FromRoute] int commentId,[FromQuery] RequestFilters filters, CancellationToken cancellationToken)
		{
			var response = await _commentService.GetCommentWithRepliesAsync(postId, commentId, filters, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
		[HttpDelete("{commentId}")]
		public async Task<IActionResult> DeleteComment([FromRoute] int commentId, CancellationToken cancellationToken)
		{
			var response = await _commentService.DeleteCommentAsync(User.GetUserId()!, commentId, cancellationToken);
			return response.IsSuccess ? Ok() : response.ToProblem();
		}
	}
}
