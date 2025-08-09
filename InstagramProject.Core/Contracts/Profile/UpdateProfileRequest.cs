using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
    public record UpdateProfileRequest
    (
        string? FullName,
        string? UserName,
        string? Bio,
        string? Password,
		IFormFile? ProfilePic
    );
}
