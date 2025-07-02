using InstagramProject.Core.Contracts.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
	public interface IPostService
	{
		Task<Result<PostResponse>> CreatePostAsunc(CreatePostRequest request, CancellationToken cancellationToken);
	}
}
