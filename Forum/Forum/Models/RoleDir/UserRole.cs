using Forum.Models.UserDir;
using Microsoft.AspNetCore.Identity;

namespace Forum.Models.RoleDir
{
	public class UserRole : IdentityUserRole<Guid>
	{
		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}