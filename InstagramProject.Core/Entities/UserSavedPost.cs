using InstagramProject.Core.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities
{
    public class UserSavedPost
     {
        public string UserId { get; set; } = string.Empty;
        public DateTime? SavedOn { get; set; } = DateTime.UtcNow;
        public ApplicationUser User { get; set; } = null!;
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

      }
}
