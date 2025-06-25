using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Entities.Auth
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string? Bio { get; set; } = string.Empty;


        public string? ProfilePic { get; set; }

        public DateOnly BirthDay { get; set; }
        public DateOnly? JoinedOn { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; } = [];

        //public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

    }
}
