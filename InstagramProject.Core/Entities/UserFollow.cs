using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
    public class UserFollow
    {
        public string UserId { get; set; } = string.Empty;
        public string FollowId { get; set; } = string.Empty;
        public DateTime FollowedOn { get; set; }

        public ApplicationUser Follower { get; set; } = null!;
        public ApplicationUser FollowedUser { get; set; } = null!;
    }
}
