using InstagramProject.Core.Contracts.UserFollow;
using InstagramProject.Core.Extensions;
using InstagramProject.Core.Service_contract;
using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProfileController : ControllerBase
	{
		private readonly IProfileService _profileService;
		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}
		[HttpPost("")]
		public async Task<IActionResult> AddFollower([FromBody] AddUserFollowRequest request, CancellationToken cancellationToken)
		{
			var response = await _profileService.AddUserFollowAsync(User.GetUserId()!, request.FollowedUserId, cancellationToken);
			return response.IsSuccess ? Ok() : response.ToProblem();
		}
		[HttpDelete("")]
		public async Task<IActionResult> DeleteFollowRelation([FromBody] AddUserFollowRequest request, CancellationToken cancellationToken)
		{
			var response = await _profileService.DeleteUserFollowAsync(User.GetUserId()!, request.FollowedUserId, cancellationToken);
			return response.IsSuccess ? NoContent() : response.ToProblem();
		}
	}
}
