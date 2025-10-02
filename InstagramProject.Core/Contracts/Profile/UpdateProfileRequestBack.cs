using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public record UpdateProfileRequestBack
    (
         string? FullName,
         string? UserName,
         string? Bio,
         string? ProfilePic
    );
}
