﻿using InstagramProject.Core.Abstractions;
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
		string? ProfileImage,
		string Content,
		int NumberOfReplies,
		DateTime Time
	);
}
