using System.Security.Claims;
using Forum.Exceptions;
using Forum.Models;
using Forum.Models.Data;
using Forum.Models.SectionForumDir;
using Forum.Models.UserDir;

namespace Forum.Services
{
	public interface IForumService
	{
		List<ForumDto> GetAll();
		Task CreateForum(ForumModel model);
		Task Edit(Guid id, ForumModel model, IEnumerable<Claim> userClaims);
		Task Delete(Guid id);
	}

	public class ForumService : IForumService
	{
		private readonly ApplicationDbContext _context;

		public ForumService(ApplicationDbContext context)
		{
			_context = context;
		}

		public List<ForumDto> GetAll()
		{
			return _context.ForumSections.Select(x => new ForumDto
			{
				Id = x.ForumId,
				Name = x.Name,
				Descripton = x.Description,
			}).ToList();
		}

		public async Task CreateForum(ForumModel model)
		{
			ForumSection forum = new ForumSection()
			{
				Description = model.Description,
				Name = model.Name
			};

			_context.ForumSections.Add(forum);
			await _context.SaveChangesAsync();
		}

		public async Task Edit(Guid id, ForumModel model, IEnumerable<Claim> userClaims)
		{
			Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

			Claim AdminClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Administrator);

			ForumSection Forum = _context.ForumSections.Find(id);
			if (Forum == null)
			{
				throw new ObjectNotFoundException("Forum not found");
			}

			User user = _context.Users.Find(userId);
			if (user == null)
			{
				throw new ObjectNotFoundException("User not found");
			}

			if (AdminClaims == null)
			{
				UserForumSection userForumSection = _context.UserForumSection
					.Where(x => x.ForumSectionId == Forum.ForumId && x.UserId == userId).FirstOrDefault();
				if (userForumSection == null)
				{
					throw new NotPermissionException("Havent permissions");
				}
			}


			Forum.Name = model.Name;
			Forum.Description = model.Description;
			await _context.SaveChangesAsync();
		}

		public async Task Delete(Guid id)
		{
			ForumSection Forum = _context.ForumSections.Find(id);

			if (Forum == null)
			{
				throw new ObjectNotFoundException("Element not found");
			}

			_context.ForumSections.Remove(Forum);

			await _context.SaveChangesAsync();
		}
	}
}