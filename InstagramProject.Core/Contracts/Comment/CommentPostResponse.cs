using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Comment
{
	public record CommentPostResponse
	(
		string UserId,
		string UserName,
		string Content,
		DateTime Time
	);
}
