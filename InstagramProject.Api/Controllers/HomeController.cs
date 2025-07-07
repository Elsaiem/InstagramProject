using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Extensions;
using InstagramProject.Core.Service_contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstagramProject.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class HomeController : ControllerBase
	{
		private readonly IHomeService _homeService;
		public HomeController(IHomeService homeService)
		{
			_homeService = homeService;
		}
	[HttpGet("")]
	public async Task<IActionResult> GetUserFeed([FromQuery] RequestFilters request, CancellationToken cancellationToken = default)
	{
		var response = await _homeService.UserFeedAsync(User.GetUserId()!, request, cancellationToken);
		return Ok(response);
	}
		[HttpGet("search")]
		public async Task<IActionResult> SearchForUser([FromQuery] SearchRequest request, CancellationToken cancellationToken)
		{
			var response = await _homeService.SearchForUserAdync(request, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
	}
}
