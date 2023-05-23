using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Forum.Models.AttachmentDir;
using Forum.Models.MessageDir;
using Forum.Services;

namespace Forum.Controllers
{
	[ApiController]
	public class MessageController : ControllerBase
	{
		public IMessageService service;

		public MessageController(IMessageService service)
		{
			this.service = service;
		}


		[Route("topics/{id}/messages")]
		[HttpGet]
		public ActionResult<List<MessageDto>> MessageGet(int id)
		{
			return Ok(service.GetAll(id));
		}

		[Route("topics/{id}/messages")]
		[HttpPost]
		[Authorize(Policy = "UserClaims")]
		public async Task<ActionResult> MessageCreate(int id, MessageModel model)
		{
			await service.Create(id, model,
				Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
			return Ok();
		}

		[Route("messages/{id}")]
		[HttpPut]
		[Authorize(Policy = "UserClaims")]
		public async Task<ActionResult> Put(int id, MessageModel model)
		{
			await service.Edit(id, model, User.Claims);
			return Ok();
		}


		[Route("messages/{id}")]
		[HttpDelete]
		[Authorize(Policy = "UserClaims")]
		public async Task<ActionResult> Delete(int id)
		{
			await service.Delete(id, User.Claims);
			return Ok();
		}


		[Route("messages/{id}/attachment")]
		[HttpPost]
		[Authorize(Policy = "UserClaims")]
		public async Task<ActionResult> CreateAttachment(int id, AttachmentModel model)
		{
			await service.AddAttachment(id, model, User.Claims);
			return Ok();
		}
	}
}