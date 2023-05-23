namespace Forum.Exceptions
{
	public class NotPermissionException : Exception
	{
		public NotPermissionException(string message) : base(message)
		{
		}
	}
}