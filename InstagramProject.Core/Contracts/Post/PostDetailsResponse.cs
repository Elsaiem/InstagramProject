using InstagramProject.Core.Contracts.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	public record PostDetailsResponse
	(
		int PostId,
		string UserId,
		string UserName,
		DateTime Time,
		string? Content,
		IEnumerable<PostMedia> Media,
		IEnumerable<CommentPostResponse> Comments
	);
}
