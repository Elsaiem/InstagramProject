using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Reaction
{
	public record CreateReactionRequest
	(
		int PostId,
		int? CommentId
	);
}
