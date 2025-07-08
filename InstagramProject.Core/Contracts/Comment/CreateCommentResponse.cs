using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Comment
{
	public record CreateCommentResponse
	(
		int CommentId,
		string Content,
		int PostId,
		int? ParentCommentId,
		string UserId,
		string UserName,
		DateTime Time,
		int RepliesCount = 0
	);
}
