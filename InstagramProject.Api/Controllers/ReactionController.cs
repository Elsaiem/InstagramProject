using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Contracts.Reaction;
using InstagramProject.Core.Extensions;
using InstagramProject.Core.Service_contract;
using InstagramProject.Core.ServiceContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InstagramProject.Core.Abstractions;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReactionController : ControllerBase
	{
		private readonly IReactionService _reactionService;
		public ReactionController(IReactionService reactionService)
		{
			_reactionService = reactionService;
		}
		[HttpPost("")]
		public async Task<IActionResult> CreateReact([FromBody] CreateReactionRequest request, CancellationToken cancellationToken)
		{
			var response = await _reactionService.CreateReactionAsync(User.GetUserId()!, request, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
	}
}
