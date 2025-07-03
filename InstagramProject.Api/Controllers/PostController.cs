using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Service_contract;
using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly IPostService _postService;
		public PostController(IPostService postService)
		{
			_postService = postService;
		}
		[HttpPost("")]
		public async Task<IActionResult> CreatePost([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
		{
			var response = await _postService.CreatePostAsunc(request, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
		[HttpGet("{postId}")]
		public async Task<IActionResult> GetPostDetails([FromRoute] int postId, CancellationToken cancellationToken)
		{
			var response = await _postService.GetPostAsync(postId, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
		[HttpPut("")]
		public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest request, CancellationToken cancellationToken)
		{
			var response = await _postService.UpdatePostAsync(request, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
		[HttpDelete("")]
		public async Task<IActionResult> DeletePost([FromBody] DeletePostRequest request, CancellationToken cancellationToken)
		{
			var response = await _postService.DeletePostAsync(request, cancellationToken);
			return response.IsSuccess ? NoContent() : response.ToProblem();
		}
	}
}
