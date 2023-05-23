using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Forum.Exceptions;
using Forum.Models;
using Forum.Models.AttachmentDir;
using Forum.Models.Data;
using Forum.Models.MessageDir;
using Forum.Models.SectionForumDir;
using Forum.Models.TopicDir;
using Forum.Models.UserDir;

namespace Forum.Services
{
	public interface IMessageService
	{
		List<MessageDto> GetAll(int id);
		Task Create(int id, MessageModel model, Guid userId);

		Task Edit(int id, MessageModel model, IEnumerable<Claim> userClaims);
		Task Delete(int id, IEnumerable<Claim> userClaims);

		Task AddAttachment(int id, AttachmentModel model, IEnumerable<Claim> userClaims);
	}

	public class MessageService : IMessageService
	{
		private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;
		private readonly IWebHostEnvironment _environment;


		public MessageService(ApplicationDbContext context, UserManager<User> userManager,
			IWebHostEnvironment environment)
		{
			_context = context;
			_userManager = userManager;
			_environment = environment;
		}


		public List<MessageDto> GetAll(int id)
		{
			List<Message> MessagesDb = _context.Messages.Where(x => x.Topic.TopicId == id).ToList();

			List<MessageDto> messagesDto = new List<MessageDto>();


			foreach (var messageDB in MessagesDb)
			{
				MessageDto messageDto = new MessageDto()
				{
					attachments = new List<string>(),
					created = messageDB.Created,
					modified = messageDB.Modified,
					id = messageDB.MessageId,
					text = messageDB.Text,
				};

				foreach (var attachments in messageDB.Attachments)
				{
					if (System.IO.File.Exists($"wwwroot/{attachments.Path}"))
					{
						Byte[] bytes = File.ReadAllBytes(attachments.Path);
						string fileBase64 = Convert.ToBase64String(bytes);
						messageDto.attachments.Add(fileBase64);
					}
				}

				messagesDto.Add(messageDto);
			}

			return messagesDto;
		}

		public async Task Create(int id, MessageModel model, Guid userId)
		{
			Topic topic = _context.Topics.Find(id);

			if (topic == null)
			{
				throw new ObjectNotFoundException("Topic not found");
			}

			User user = await _userManager.FindByIdAsync(userId.ToString());

			if (user == null)
			{
				throw new ObjectNotFoundException("User not found");
			}


			Message message = new Message()
			{
				Created = DateTime.Now,
				Modified = null,
				Text = model.text,
				Topic = topic,
				User = user,
				Attachments = new List<Attachment>()
			};

			_context.Messages.Add(message);
			await _context.SaveChangesAsync();
		}


		public async Task Edit(int id, MessageModel model, IEnumerable<Claim> userClaims)
		{
			Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


			Claim AdminClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Administrator);
			Claim ModerClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Moderator);


			Message message = _context.Messages.Include(x => x.Topic).Include(x => x.User).Where(x => x.MessageId == id)
				.FirstOrDefault();
			if (message == null)
			{
				throw new ObjectNotFoundException("Message not found");
			}


			User user = _context.Users.Find(userId);
			if (user == null)
			{
				throw new ObjectNotFoundException("User not found");
			}

			var topic = _context.Topics.Include(x => x.Forum).Where(x => x.TopicId == message.Topic.TopicId)
				.FirstOrDefault();


			if (user.Id != message.User.Id)
			{
				if (AdminClaims == null)
				{
					UserForumSection userForumSection = _context.UserForumSection
						.Where(x => x.ForumSectionId == topic.Forum.ForumId && x.UserId == userId).FirstOrDefault();
					if (userForumSection == null)
					{
						throw new NotPermissionException("Havent permissions");
					}
				}
			}


			message.Text = model.text;
			message.Modified = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		public async Task Delete(int id, IEnumerable<Claim> userClaims)
		{
			Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


			Claim AdminClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Administrator);
			Claim ModerClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Moderator);


			Message message = _context.Messages.Include(x => x.Topic).Include(x => x.User).Where(x => x.MessageId == id)
				.FirstOrDefault();
			if (message == null)
			{
				throw new ObjectNotFoundException("Message not found");
			}


			User user = _context.Users.Find(userId);
			if (user == null)
			{
				throw new ObjectNotFoundException("User not found");
			}


			ForumSection forum = _context.ForumSections.Find(message.Topic.Forum);
			if (forum == null)
			{
				throw new ObjectNotFoundException("Forum not found");
			}


			if (user.Id != message.User.Id)
			{
				if (AdminClaims == null)
				{
					UserForumSection userForumSection = _context.UserForumSection
						.Where(x => x.ForumSectionId == forum.ForumId && x.UserId == userId).FirstOrDefault();
					if (userForumSection == null)
					{
						throw new NotPermissionException("Havent permissions");
					}
				}
			}


			_context.Messages.Remove(message);
			await _context.SaveChangesAsync();
		}

		public async Task AddAttachment(int id, AttachmentModel model, IEnumerable<Claim> userClaims)
		{
			Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


			User user = await _userManager.FindByIdAsync(userId.ToString());

			if (user == null)
			{
				throw new ObjectNotFoundException("User not found");
			}


			Message message = _context.Messages.Include(x => x.User).Where(x => x.MessageId == id).FirstOrDefault();
			if (message == null)
			{
				throw new ObjectNotFoundException("Message not found");
			}


			if (message.User.Id != user.Id)
			{
				throw new NotPermissionException("Havent permissions");
			}


			Attachment attachment = new Attachment()
			{
				Message = message,
			};


			string pt = Path.GetRandomFileName().Replace(".", "");
			string FileName = pt.Substring(0, 8); // Return 8 character string
			FileName += ".jpg";


			var fileNameWithPath = string.Empty;


			fileNameWithPath = $"Files/{Guid.NewGuid()}-{FileName}";

			Byte[] bytes = Convert.FromBase64String(model.attachment);

			string path = Path.Combine(_environment.WebRootPath, fileNameWithPath);

			File.WriteAllBytes(path, bytes);


			message.Modified = DateTime.Now;
			attachment.FileName = FileName;
			attachment.Path = fileNameWithPath;

			_context.Attachments.Add(attachment);
			await _context.SaveChangesAsync();
		}
	}
}