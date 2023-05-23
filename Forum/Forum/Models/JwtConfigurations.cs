using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Forum.Models
{
	public class JwtConfigurations
	{
		public const string Issuer = "JwtTestIssuer";
		public const string Audience = "JwtTestClient";
		private const string Key = "SuperSecretKeyBazingaLolKek!*228322";
		public const int Lifetime = 1;

		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
		}
	}
}