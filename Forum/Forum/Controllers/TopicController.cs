using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Forum.Models.TopicDir;
using Forum.Services;

namespace Forum.Controllers
{
	[ApiController]
	public class TopicController : ControllerBase
	{
		public ITopicService service;

		public TopicController(ITopicService service)
		{
			this.service = service;
		}

		[Route("sections/{sectionId}/topics")]
		[HttpGet]
		public ActionResult<List<TopicDto>> TopicGet(Guid sectionId)
		{
			return Ok(service.GetAll(sectionId));
		}


		[Route("sections/{sectionId}/topics")]
		[HttpPost]
		[Authorize(Policy = "UserClaims")]
		public async Task<IActionResult> Create(Guid sectionId, TopicModel model)
		{
			if (ModelState.IsValid)
			{
				await service.CreateTopic(sectionId, model,
					Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
				return Ok();
			}
			else
			{
				throw new ValidationException("Bad data");
			}
		}


		[Route("topics/{topicId}")]
		[HttpPut]
		[Authorize(Policy = "AdminModeratorUserClaims")]
		public async Task<IActionResult> Put(int topicId, TopicModel model)
		{
			if (!ModelState.IsValid)
			{
				throw new ValidationException("Bad data");
			}

			await service.Edit(topicId, model, User.Claims);
			return Ok();
		}


		[Route("topics/{topicId}")]
		[HttpDelete]
		[Authorize(Policy = "AdminModeratorUserClaims")]
		public async Task<IActionResult> Delete(int topicId)
		{
			await service.Delete(topicId, User.Claims);
			return Ok();
		}
	}
}