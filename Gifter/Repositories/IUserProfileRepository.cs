using Gifter.Models;

namespace Gifter.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile user);
        void Delete(int id);
        void Update(UserProfile user);
        UserProfile GetUserProfileWithPosts(int userProfileId);
    }
}