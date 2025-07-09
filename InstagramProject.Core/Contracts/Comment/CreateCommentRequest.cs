using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Comment
{
	public record CreateCommentRequest
	(
		int PostId,
		string Content,
		int? ParentCommentId = null
	);
}
