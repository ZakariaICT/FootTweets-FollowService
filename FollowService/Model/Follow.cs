using System.ComponentModel.DataAnnotations;

namespace FollowService.Model
{
    public class Follow
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]

        public string FollowerId { get; set; }
        [Required]

        public string FollowingId { get; set; }

        //public string FollowerUsername { get; set; }

        //public string FollowingUsername { get; set; }
    }
}
