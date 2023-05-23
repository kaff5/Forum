using Forum.Models.RoleDir;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models.UserDir
{
	public class User : IdentityUser<Guid>
	{
		public string Name { get; set; }

		public ICollection<UserRole> Roles { get; set; }

		public ICollection<UserForumSection> ForumSections { get; set; }
	}
}