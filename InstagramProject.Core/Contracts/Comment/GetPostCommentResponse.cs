using InstagramProject.Core.Contracts.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	public record GetPostCommentResponse
	(
		int PostId,
		IEnumerable<CommentPostResponse> Comments
	);
}
