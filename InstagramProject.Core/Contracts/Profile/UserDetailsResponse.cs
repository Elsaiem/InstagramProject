using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Profile
{
	public record UserDetailsResponse
	(
		string userId,
		string userName,
		string fullName,
		string profilePic,
		string bio,
		int followersCount,
		int followingCount,
		int postsCount,
		bool isFollowing,
		bool isCurrentUser,
		bool isPrivate,
		IEnumerable<UserPosts> posts
	);
}
