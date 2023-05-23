using Forum.Middleware;
using Forum.Models;
using Forum.Models.Data;
using Forum.Models.RoleDir;
using Forum.Models.UserDir;
using Forum.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IForumService, ForumService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, Role>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager<SignInManager<User>>()
	.AddUserManager<UserManager<User>>()
	.AddRoleManager<RoleManager<Role>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = JwtConfigurations.Issuer,
			ValidateAudience = true,
			ValidAudience = JwtConfigurations.Audience,
			ValidateLifetime = false,
			IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
			ValidateIssuerSigningKey = true,
		};
	});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("UserClaims", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("Role", "Пользователь");
	});
	options.AddPolicy("AdminClaims", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("Role", "Администратор");
	});
	options.AddPolicy("ModeratorClaims", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("Role", "Модератор");
	});
	options.AddPolicy("AdminModeratorClaims", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("Role", "Модератор", "Администратор");
	});
	options.AddPolicy("AdminModeratorUserClaims", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("Role", "Модератор", "Администратор", "Пользователь");
	});
});


var app = builder.Build();

app.UseExceptionHandlingMiddlewares();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();