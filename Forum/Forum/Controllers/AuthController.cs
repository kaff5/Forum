using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Forum.Exceptions;
using Forum.Models;
using Forum.Models.Auth;
using Forum.Models.Data;
using Forum.Models.UserDir;
using Forum.Services;

namespace Forum.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAuthService service;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public AuthController(IAuthService service, ApplicationDbContext context, UserManager<User> userManager)
        {
            this.service = service;
            _context = context;
            _userManager = userManager;
        }



        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<TokenModel>> login(LoginDto model)
        {
            var identity = await GetIdentity(model.userName, model.password);
            if (identity == null)
            {
                throw new Exception("Invalid username or password.");
            }


            var token = CreateToken(identity);

            var response = new TokenModel
            {
                Token = token,
            };

            return new JsonResult(response);
        }

        private static string CreateToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                issuer: JwtConfigurations.Issuer,
                audience: JwtConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                 //expires: now.Add(JwtConfigurations.Lifetime),
                 expires: now.Add(TimeSpan.FromMinutes(JwtConfigurations.Lifetime)),
                signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }





        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                throw new ValidationException("Wrong username or password");
            }
            else
            {
                var check = _userManager.CheckPasswordAsync(user, password);

                if (check.IsCompleted)
                {
                    List<IdentityUserClaim<Guid>> claimsDB = _context.UserClaims.Where(x => x.UserId == user.Id).ToList();




                    if (!claimsDB.Any())
                    {
                        var identity = new IdentityUserClaim<Guid>
                        {
                            UserId = user.Id,
                            ClaimType = "Role",
                            ClaimValue = ClaimsValueStr.User

                        };
                        _context.UserClaims.Add(identity);
                    }
                    await _context.SaveChangesAsync();

                    List<Claim> claims = new List<Claim>();
                    claimsDB = _context.UserClaims.Where(x => x.UserId == user.Id).ToList();
                    foreach (var claim in claimsDB)
                    {
                        claims.Add(new Claim(claim.ClaimType, claim.ClaimValue));
                    }

                    claims.Add(new Claim("Name", user.Name));

                    claims.Add(new("Id", user.Id.ToString()));
                    claims.Add(new(ClaimTypes.NameIdentifier, user.Id.ToString()));




                    //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
                    var claimsIdentity =
                        new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                else throw new ValidationException("Wrong username or password");
            }

        }


        [Route("logout")]
        [HttpPost]
        [Authorize(Policy = "ClaimRole")]
        public async Task<IActionResult> Logout()
        {
            //await service.Logout(User.Identity.Name);
            return Ok();
        }

        [Route("register")]
        [HttpPost]
        //[Authorize(Policy = "Пользователь")]
        public async Task<ActionResult<TokenModel>> Register([FromBody] RegisterDto model)
        {
/*            if (!ModelState.IsValid) //Проверка полученной модели данных
            {
                throw new ValidationException("Bad data");
            }*/
            await service.Register(model);
            return await login(new LoginDto
            {
                userName = model.name,
                password = model.Password
            });
        }
    }
}
