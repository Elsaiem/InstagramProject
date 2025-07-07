using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Errors.ProfileError
{
    public static class ProfileErrors
    {
        // ... existing code ...
        public static readonly Error FollowRequestNotFound = new("Profile.FollowRequestNotFound", "Follow request not found", StatusCodes.Status404NotFound);
        public static readonly Error FollowRequestAlreadyExists = new("Profile.FollowRequestAlreadyExists", "A follow request already exists", StatusCodes.Status400BadRequest);
        public static readonly Error CannotFollowYourself = new("Profile.CannotFollowYourself", "You cannot follow yourself", StatusCodes.Status400BadRequest);
    }
}
