using InstagramProject.Core.Abstractions;
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
		int CommentId,
		string UserName,
		string? ProfileImage,
		string Content,
		int NumberOfReplies,
		int NumberOfReactions,
		bool IsReacted,
		DateTime Time
	);
}
