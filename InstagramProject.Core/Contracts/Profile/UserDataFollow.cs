using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public record UserDataFollow
    {

        public string UserId { get; init; }
        public string FullName { get; init; }
        public string? ProfilePic { get; init; }
        public bool IsFollow { get; init; }
        public DateTime? followedOn { get; init; } = DateTime.Now;

    }
}
