using InstagramProject.Core.Abstractions;
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
		Task<Result<PostDetailsResponse>> GetPostAsync(int postId, CancellationToken cancellationToken);
		Task<Result<PostResponse>> UpdatePostAsync(UpdatePostRequest request, CancellationToken cancellationToken);
		Task<Result> DeletePostAsync(DeletePostRequest request, CancellationToken cancellationToken);
	}
}
