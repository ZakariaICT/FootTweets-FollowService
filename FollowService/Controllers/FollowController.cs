﻿using FollowService.Data;
using FollowService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FollowService.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class FollowController : Controller
    {
        private readonly AppDBContext appDBContext;

        public FollowController (AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        [HttpPost("follow")]
        public IActionResult FollowUser([FromBody] Follow follow)
        {
            // Validate and process the follow action
            if (follow == null || string.IsNullOrEmpty(follow.FollowerId) || string.IsNullOrEmpty(follow.FollowingId))
            {
                return BadRequest("Invalid follow data");
            }

            // Check if the follow relationship already exists
            if (appDBContext.follows.Any(f => f.FollowerId == follow.FollowerId && f.FollowingId == follow.FollowingId))
            {
                return Conflict("Already following the user");
            }

            appDBContext.follows.Add(follow);
            appDBContext.SaveChanges();

            return Ok("Followed successfully");
        }

        [HttpPost("unfollow")]
        public IActionResult UnfollowUser([FromBody] Follow follow)
        {
            // Validate and process the unfollow action
            if (follow == null || string.IsNullOrEmpty(follow.FollowerId) || string.IsNullOrEmpty(follow.FollowingId))
            {
                return BadRequest("Invalid unfollow data");
            }

            var existingFollow = appDBContext.follows
                .FirstOrDefault(f => f.FollowerId == follow.FollowerId && f.FollowingId == follow.FollowingId);

            if (existingFollow == null)
            {
                return NotFound("Follow relationship not found");
            }

            appDBContext.follows.Remove(existingFollow);
            appDBContext.SaveChanges();

            return Ok("Unfollowed successfully");
        }

        [HttpGet("{userId}")]
        public IActionResult GetFollowers(string userId)
        {
            var followers = appDBContext.follows
                .Where(f => f.FollowingId == userId)
                .Select(f => f.FollowerId)
                .ToList();

            return Ok(followers);
        }
    }
}
