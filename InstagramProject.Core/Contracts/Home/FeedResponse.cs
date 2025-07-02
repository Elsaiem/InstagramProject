using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Home
{
	public record FeedResponse
	(
		string userId,
		string userName,
		IEnumerable<FeedPostResponse> posts,
		int Likes,
		int Comments
	);
}
