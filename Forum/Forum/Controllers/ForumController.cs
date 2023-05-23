using Forum.Models.SectionForumDir;
using Forum.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.Controllers
{
	[Route("sections")]
	[ApiController]
	public class ForumController : ControllerBase
	{
		public IForumService service;

		public ForumController(IForumService service)
		{
			this.service = service;
		}


		[HttpGet]
		public ActionResult<List<ForumDto>> SectionsGet()
		{
			return Ok(service.GetAll());
		}


		[HttpPost]
		[Authorize(Policy = "AdminClaims")]
		public async Task<IActionResult> Create(ForumModel model)
		{
			await service.CreateForum(model);
			return Ok();
		}


		[HttpPut("{id}")]
		[Authorize(Policy = "AdminModeratorClaims")]
		public async Task<IActionResult> Put(Guid id, ForumModel model)
		{
			await service.Edit(id, model, User.Claims);
			return Ok();
		}

		[HttpDelete("{id}")]
		[Authorize(Policy = "AdminClaims")]
		public async Task<IActionResult> Delete(Guid id)
		{
			await service.Delete(id);
			return Ok();
		}
	}
}