using Microsoft.AspNetCore.Identity;

namespace Forum.Models.RoleDir
{
	public class Role : IdentityRole<Guid>
	{
		public int RoleId { get; set; }
		public string Name { get; set; }

		public ICollection<UserRole> Users { get; set; }
	}
}