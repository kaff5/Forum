using Forum.Models;
using Forum.Models.UserDir;
using Forum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
	[ApiController]
	public class UserController : ControllerBase
	{
		public IUserService service;

		public UserController(IUserService service)
		{
			this.service = service;
		}

		[Route("users")]
		[HttpGet]
		[Authorize(Policy = "AdminClaims")]
		public ActionResult<List<UsersDto>> Get()
		{
			return Ok(service.GetAll());
		}


		[Route("users/addForumModer")]
		[HttpPost]
		[Authorize(Policy = "AdminClaims")]
		public ActionResult PostForum(PostForumModel model)
		{
			service.PostForum(model);
			return Ok();
		}

		[Route("users/{userId}/addClaimAdmin")]
		[HttpPost]
		[Authorize(Policy = "AdminClaims")]
		public ActionResult PostClaim(Guid userId)
		{
			service.PostClaim(userId);
			return Ok();
		}
	}
}