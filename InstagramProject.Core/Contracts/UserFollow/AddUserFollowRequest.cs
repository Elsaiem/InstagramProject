using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.UserFollow
{
	public record AddUserFollowRequest
	(
		string FollowedUserId
	);
}
