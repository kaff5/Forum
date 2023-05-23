using Forum.Models.AttachmentDir;
using Forum.Models.MessageDir;
using Forum.Models.RoleDir;
using Forum.Models.SectionForumDir;
using Forum.Models.TopicDir;
using Forum.Models.UserDir;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forum.Models.Data
{
	public class ApplicationDbContext
		: IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>,
			IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		public override DbSet<User> Users { get; set; }
		public override DbSet<Role> Roles { get; set; }
		public override DbSet<UserRole> UserRoles { get; set; }
		public DbSet<ForumSection> ForumSections { get; set; }
		public DbSet<Topic> Topics { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Attachment> Attachments { get; set; }
		public DbSet<UserForumSection> UserForumSection { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>(o => { o.ToTable("Users"); });

			builder.Entity<Role>(o => { o.ToTable("Roles"); });

			builder.Entity<UserRole>(o =>
			{
				o.ToTable("UserRoles");
				o.HasOne(x => x.Role)
					.WithMany(x => x.Users)
					.HasForeignKey(x => x.RoleId)
					.OnDelete(DeleteBehavior.Cascade);
				o.HasOne(x => x.User)
					.WithMany(x => x.Roles)
					.HasForeignKey(x => x.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<ForumSection>(o =>
			{
				o.HasMany(x => x.Topics).WithOne(x => x.Forum).OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Topic>(o =>
			{
				o.HasMany(x => x.Messages).WithOne(x => x.Topic).OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Message>(o =>
			{
				o.HasMany(x => x.Attachments).WithOne(x => x.Message).OnDelete(DeleteBehavior.Cascade);
				o.HasOne(x => x.User).WithMany().OnDelete(DeleteBehavior.NoAction);
			});

			builder.Entity<UserForumSection>(o =>
			{
				o.ToTable("UserForumSection");

				o.HasKey(x => new { x.UserId, x.ForumSectionId });
				o.HasOne(x => x.ForumSection)
					.WithMany(x => x.Users)
					.HasForeignKey(x => x.ForumSectionId)
					.OnDelete(DeleteBehavior.NoAction);
				o.HasOne(x => x.User)
					.WithMany(x => x.ForumSections)
					.HasForeignKey(x => x.UserId)
					.OnDelete(DeleteBehavior.NoAction);
			});
		}
	}
}