using Forum.Models.SectionForumDir;
using Forum.Models.UserDir;

namespace Forum.Models
{
	public class UserForumSection
	{
		public Guid UserId { get; set; }
		public User User { get; set; }
		public Guid ForumSectionId { get; set; }
		public ForumSection ForumSection { get; set; }
	}
}