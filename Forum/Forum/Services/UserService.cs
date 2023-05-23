using Forum.Exceptions;
using Forum.Models;
using Forum.Models.Data;
using Forum.Models.SectionForumDir;
using Forum.Models.UserDir;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Services
{
	public interface IUserService
	{
		public List<UsersDto> GetAll();
		Task PostClaim(Guid userId);
		Task PostForum(PostForumModel model);
	}

	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;

		public UserService(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		public List<UsersDto> GetAll()
		{
			List<UsersDto> users = _context.Users.Include(x => x.ForumSections).Select(x => new UsersDto
			{
				Id = x.Id,
				Name = x.UserName,
				Forums = x.ForumSections.Select(o => new ForumDto
				{
					Id = o.ForumSectionId,
					Descripton = o.ForumSection.Description,
					Name = o.ForumSection.Name
				}).ToList(),
			}).ToList();


			foreach (var user in users)
			{
				List<IdentityUserClaim<Guid>> claims = _context.UserClaims.Where(x => x.UserId == user.Id).ToList();
				user.Claims = new List<ClaimsDto>();


				foreach (var claim in claims)
				{
					user.Claims.Add(new ClaimsDto
					{
						Type = claim.ClaimType,
						Value = claim.ClaimValue
					});
				}
			}


			return users;
		}


		public async Task PostClaim(Guid userId)
		{
			var user = _context.Users.Find(userId);
			if (user is null)
			{
				throw new ValidationException("Wrong UserId");
			}
			else
			{
				var identity = new IdentityUserClaim<Guid>
				{
					UserId = user.Id,
					ClaimType = "Role",
					ClaimValue = ClaimsValueStr.Administrator
				};
				_context.UserClaims.Add(identity);


				await _context.SaveChangesAsync();
			}
		}


		public async Task PostForum(PostForumModel model)
		{
			var user = _context.Users.Find(model.UserId);
			if (user is null)
			{
				throw new ValidationException("Wrong UserId");
			}

			user.ForumSections = new List<UserForumSection>();

			var claim = _context.UserClaims.Where(x => x.UserId == model.UserId && x.ClaimValue.Contains("Модератор"));
			if (claim.Count() == 0)
			{
				var identity = new IdentityUserClaim<Guid>
				{
					UserId = user.Id,
					ClaimType = "Role",
					ClaimValue = ClaimsValueStr.Moderator
				};
				_context.UserClaims.Add(identity);
			}

			ForumSection forum = _context.ForumSections.Find(model.ForumId);
			if (forum == null)
			{
				throw new ValidationException("Wrong ForumId");
			}

			UserForumSection userForumSection = new UserForumSection()
			{
				ForumSection = forum,
				User = user
			};

			user.ForumSections.Add(userForumSection);

			await _context.SaveChangesAsync();
		}
	}
}