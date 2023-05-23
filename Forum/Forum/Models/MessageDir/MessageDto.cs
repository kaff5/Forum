namespace Forum.Models.MessageDir
{
	public class MessageDto
	{
		public int id { get; set; }
		public string text { get; set; }

		public List<string> attachments { get; set; }

		public DateTime created { get; set; }

		public DateTime? modified { get; set; }
	}
}