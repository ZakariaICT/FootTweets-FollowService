using FollowService.Data;
using Microsoft.EntityFrameworkCore;
using FollowService.Data;

namespace FollowService.Repositories
{
    public class FollowRepo : IFollowRepo
    {
        private readonly AppDBContext _context;

        public FollowRepo(AppDBContext context)
        {
            _context = context;
        }

        public void DeleteFollowContentById(string followId)
        {
            var followToDelete = _context.follows.Where(p => p.FollowerId == followId).ToList();

            foreach (var following in  followToDelete)
            {
                _context.follows.Remove(following);
            }

            _context.SaveChanges();
        }


        public bool saveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
