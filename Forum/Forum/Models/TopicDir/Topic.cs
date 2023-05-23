using Forum.Models.MessageDir;
using Forum.Models.SectionForumDir;
using Forum.Models.UserDir;

namespace Forum.Models.TopicDir
{
	public class Topic
	{
		public int TopicId { get; set; }
		public string Name { get; set; }

		public string Description { get; set; }

		public DateTime Created { get; set; }

		public ForumSection Forum { get; set; }


		public ICollection<Message> Messages { get; set; }

		public User User { get; set; }
	}
}