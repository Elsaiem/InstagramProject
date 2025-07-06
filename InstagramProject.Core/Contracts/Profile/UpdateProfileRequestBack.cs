using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public record UpdateProfileRequestBack
    {
        public string? FullName { get; init; }
        public string? UserName { get; init; }
        public string? Email { get; init; }
        public string? Bio { get; init; }
        public string? Profile_Image { get; init; }
    }
}
