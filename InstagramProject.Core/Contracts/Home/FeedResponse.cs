using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Home
{
	public record FeedResponse
	(
		int PostId,
		string userId,
		string userName,
		string ProfilePic,
		string Content,
		DateTime Time,
		IEnumerable<string> media,
		int Likes,
		int Comments
	);
}
