namespace FollowService.Repositories
{
    public interface IFollowRepo
    {
        void DeleteFollowContentById(string followId);

        bool saveChanges();
    }
}
