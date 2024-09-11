using Gifter.Models;

namespace Gifter.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        void Delete(int id);
        void Update(Post post);
        Post GetById(int id);
        List<Post> GetAll();
        List<Post> GetAllWithComments();
        List<Post> Search(string criterion, bool sortDescending);
        List<Post> GetPostsSince(DateTime sinceDate); // New method for posts since a date
    }
}