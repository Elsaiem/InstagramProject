using InstagramProject.Core.Extensions;
using InstagramProject.Core.Service_contract;
using Microsoft.AspNetCore.Authorization;
using InstagramProject.Core.Abstractions;
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
		[HttpPost("")]
		public async Task<IActionResult> Login(CancellationToken cancellationToken)
		{
			var response = await _homeService.UserFeedAsync(User.GetUserId()!, cancellationToken);
			return response.IsSuccess ? Ok(response.Value) : response.ToProblem();
		}
	}
}
