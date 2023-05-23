using Forum.Exceptions;
using Forum.Models.Auth;
using Forum.Models.UserDir;
using Microsoft.AspNetCore.Identity;

namespace Forum.Services
{
	public interface IAuthService
	{
		public Task Register(RegisterDto model);
	}

	public class AuthService : IAuthService
	{
		private readonly UserManager<User> _userManager;

		public AuthService(UserManager<User> userManager)
		{
			_userManager = userManager;
		}


		public async Task Register(RegisterDto model)
		{
			var user = new User
			{
				UserName = model.name,
				Name = model.name
			};


			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				return;
			}
			else throw new ValidationException(string.Join($",  ", result.Errors.Select(x => x.Description)));
		}
	}
}