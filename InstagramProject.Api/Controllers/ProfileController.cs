using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Profile;
using InstagramProject.Core.Contracts.UserFollow;
using InstagramProject.Core.Extensions;
using InstagramProject.Core.Service_contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InstagramProject.Core.Contracts.Profile;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ProfileController : ControllerBase
	{
		private readonly IProfileService _profileService;
		public ProfileController(IProfileService profileService)
		{
			_profileService = profileService;
		}
		[HttpGet("{userName}")]
		public async Task<IActionResult> GetUserDetails(string userName, CancellationToken cancellationToken)
		{
			var result = await _profileService.GetUserDetailsAsync(userName, User.GetUserName()!, cancellationToken);
			return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
		}
		[HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
        {
            var result = await _profileService.DeleteAsync(cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
        [HttpPut("update-account")]
        public async Task<IActionResult> UpdateAccount([FromForm] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _profileService.UpdateProfileAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPut("toggle-following")]
        public async Task<IActionResult> ToggleFollowingAndFollowers(CancellationToken cancellationToken)
        {
            var result = await _profileService.ToggleFollowerAndFollowing(User.GetUserName()!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("toggle-PrivateAccount")]
        public async Task<IActionResult> togglePrivateAccount(CancellationToken cancellationToken)
        {
            var result = await _profileService.TogglePrivacyTheAccount(User.GetUserName()!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpGet("privacy")]
        public async Task<IActionResult> GetPrivacy(CancellationToken cancellationToken)
        {
            var result = await _profileService.GetPrivacyAsync(User.GetUserName()!, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("Follow")]
        public async Task<IActionResult> AddUserFollow([FromBody] string targetUserId, CancellationToken cancellationToken)
        {
            var result = await _profileService.HandleFollowActionAsync(targetUserId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("un-follow")]
        public async Task<IActionResult> DeleteUserFollow([FromBody] AddFollowRequest request, CancellationToken cancellationToken)
        {
            var result = await _profileService.DeleteUserFollowAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
        [HttpPost("accept-follow-request")]
        public async Task<IActionResult> AcceptFollowRequest([FromBody] string requesterId, CancellationToken cancellationToken)
        {
            var result = await _profileService.AcceptFollowRequestAsync(requesterId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("reject-follow-request")]
        public async Task<IActionResult> RejectFollowRequest([FromBody] string requesterId, CancellationToken cancellationToken)
        {
            var result = await _profileService.RejectFollowRequestAsync(requesterId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-all-followers/{userName}")]
        public async Task<IActionResult> GetAllUserFollowers([FromRoute] string userName, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetAllFollowers(userName, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("get-all-following/{userName}")]
        public async Task<IActionResult> GetAllUserFollowing([FromRoute] string userName, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetAllFollowing(userName, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("follow-details/{followName}")]
        public async Task<IActionResult> GetFollowDetails([FromRoute] string followName, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetFollowersDetailsAsync(User.GetUserId()!, followName, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpDelete("remove-follower")]
        public async Task<IActionResult> RemoveFollower([FromBody] RemoveFollowerRequest request, CancellationToken cancellationToken)
        {
            var result = await _profileService.RemoveFollowersAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        [HttpGet("pending-follow-requests")]
        public async Task<IActionResult> GetPendingFollowRequests(CancellationToken cancellationToken)
        {
            var result = await _profileService.GetPendingFollowRequestsAsync(cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("UserPosts/{userId}")]
        public async Task<IActionResult> GetUserPosts(string userId, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetUserPosts(userId, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("FollowStatus/{targetUserId}")]
        public async Task<IActionResult> GetFollowStatus(string targetUserId, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetFollowStatusAsync(targetUserId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem(); // assuming you have an extension method to map Result<Error> to ProblemDetails
        }


    }
}
