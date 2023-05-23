using System.ComponentModel.DataAnnotations;
using Forum.Models.TopicDir;

namespace Forum.Models.SectionForumDir
{
	public class ForumSection
	{
		[Key]
		public Guid ForumId { get; set; }

		public string Name { get; set; }
		public string Description { get; set; }

		public ICollection<Topic> Topics { get; set; }


		public ICollection<UserForumSection> Users { get; set; }
	}
}