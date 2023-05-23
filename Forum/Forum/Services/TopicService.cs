using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Forum.Exceptions;
using Forum.Models;
using Forum.Models.Data;
using Forum.Models.SectionForumDir;
using Forum.Models.TopicDir;
using Forum.Models.UserDir;

namespace Forum.Services
{

    public interface ITopicService
    {
        List<TopicDto> GetAll(Guid id);
        Task CreateTopic(Guid id, TopicModel model, Guid userId);
        Task Edit(int id, TopicModel model, IEnumerable<Claim> userClaims);
        Task Delete(int id, IEnumerable<Claim> userClaims);
    }
    public class TopicService : ITopicService
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public TopicService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<TopicDto> GetAll(Guid id)
        {

            ForumSection forum = _context.ForumSections.Find(id);

            if (forum == null)
            {
                throw new ObjectNotFoundException("Forum not found");
            }

            IQueryable<Topic> topics = _context.Topics.Where(x => x.Forum.ForumId.ToString() == id.ToString());
            List<TopicDto> result = new List<TopicDto>();

            foreach (var top in topics)
            {
                result.Add(new TopicDto
                {
                    Descritpion = top.Description,
                    Created = top.Created,
                    Id = top.TopicId,
                    Name = top.Name,
                });
            }


            return result;
        }



        public async Task CreateTopic(Guid id, TopicModel model, Guid userId)
        {
            ForumSection forum = _context.ForumSections.Find(id);

            if (forum == null)
            {
                throw new ObjectNotFoundException("Forum not found");
            }

            User user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new ObjectNotFoundException("User not found");
            }

            Topic topic = new Topic()
            {
                Description = model.Description,
                Name = model.Name,
                Created = DateTime.Now,
                Forum = forum,
                User = user
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

        }


        public async Task Edit(int id, TopicModel model, IEnumerable<Claim> userClaims)
        {

            Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

            Claim AdminClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Administrator);
            Claim ModerClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Moderator);



            Topic topic = _context.Topics.Include(x => x.Forum).Include(x=>x.User).Where(x => x.TopicId == id).FirstOrDefault();
            if (topic == null)
            {
                throw new ObjectNotFoundException("Topic not found");
            }



            User user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new ObjectNotFoundException("User not found");
            }



            if (user.Id != topic.User.Id)
            {
                if (AdminClaims == null)
                {
                    UserForumSection userForumSection = _context.UserForumSection.Where(x => x.ForumSectionId == topic.Forum.ForumId && x.UserId == userId).FirstOrDefault();
                    if (userForumSection == null)
                    {
                        throw new NotPermissionException("Havent permissions");
                    }
                }
            }

            topic.Name = model.Name;
            topic.Description = model.Description;

            await _context.SaveChangesAsync();

        }

        public async Task Delete(int id, IEnumerable<Claim> userClaims)
        {

            Guid userId = Guid.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);


            Claim AdminClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Administrator);
            Claim ModerClaims = userClaims.FirstOrDefault(x => x.Value == ClaimsValueStr.Moderator);


            Topic topic = _context.Topics.Include(x=>x.Forum).Include(x => x.User).Where(x=>x.TopicId == id).FirstOrDefault();
            if (topic == null)
            {
                throw new ObjectNotFoundException("Topic not found");
            }



            User user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new ObjectNotFoundException("User not found");
            }



            if (user.Id != topic.User.Id)
            {
                if (AdminClaims == null)
                {
                    UserForumSection userForumSection = _context.UserForumSection.Where(x => x.ForumSectionId == topic.Forum.ForumId && x.UserId == userId).FirstOrDefault();
                    if (userForumSection == null)
                    {
                        throw new NotPermissionException("Havent permissions");
                    }
                }
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

        }




    }
}
