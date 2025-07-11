﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	public record PostResponse
	(
		int PostId,
		string UserId,
		string? Content,
		IEnumerable<PostMedia> Media
	);
}
