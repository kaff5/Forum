using Forum.Models.RoleDir;
using System.Security.Claims;
using Forum.Models.SectionForumDir;

namespace Forum.Models.UserDir
{
	public class UsersDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public List<ForumDto> Forums { get; set; }

		public List<ClaimsDto> Claims { get; set; }
	}
}