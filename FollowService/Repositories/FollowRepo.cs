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
            var followToDelete = _context.follows.FirstOrDefault(f => f.FollowerId == followId);

            if (followToDelete != null)
            {
                _context.follows.Remove(followToDelete);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException($"Follow with ID {followId} not found.");
            }
        }
    }
}
