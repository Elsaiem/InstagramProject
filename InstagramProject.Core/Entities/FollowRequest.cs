using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
    public class FollowRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RequesterId { get; set; } = string.Empty;
        public string TargetUserId { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public bool IsAccepted { get; set; }

        public ApplicationUser Requester { get; set; } = null!;
        public ApplicationUser TargetUser { get; set; } = null!;
    }
}
