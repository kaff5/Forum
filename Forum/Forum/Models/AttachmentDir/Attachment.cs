using Forum.Models.MessageDir;

namespace Forum.Models.AttachmentDir
{
	public class Attachment
	{
		public int AttachmentId { get; set; }
		public string FileName { get; set; }
		public string Path { get; set; }
		public Message Message { get; set; }
	}
}