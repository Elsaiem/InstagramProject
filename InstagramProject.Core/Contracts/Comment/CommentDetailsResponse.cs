using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Comment
{
	public record CommentDetailsResponse
	(
		int CommentId,
		string Content,
		int PostId,
		int? ParentCommentId,
		string UserId,
		string UserName,
		string? UserProfilePic,
		DateTime Time,
		int LikesCount,
		bool IsLikedByUser
	);
}
