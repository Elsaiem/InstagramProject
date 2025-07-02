using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Post
{
	public record CreatePostRequest
	(
		string userId,
		string? Content,
		IEnumerable<IFormFile> PostMedia
	);
}
