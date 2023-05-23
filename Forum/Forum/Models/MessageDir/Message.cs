using Forum.Models.AttachmentDir;
using Forum.Models.TopicDir;
using Forum.Models.UserDir;

namespace Forum.Models.MessageDir
{
	public class Message
	{
		public int MessageId { get; set; }

		public string Text { get; set; }

		public DateTime Created { get; set; }
		public DateTime? Modified { get; set; }

		public ICollection<Attachment> Attachments { get; set; }

		public User User { get; set; }

		public Topic Topic { get; set; }
	}
}