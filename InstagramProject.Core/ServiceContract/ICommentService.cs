using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Comment;
using InstagramProject.Core.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.ServiceContract
{
	public interface ICommentService
	{
		Task<Result<CreateCommentResponse>> CreateCommentAsync(string userId, CreateCommentRequest request, CancellationToken cancellationToken = default);
		Task<Result<PaginatedList<CommentDetailsResponse>>> GetCommentWithRepliesAsync(int postId, int commentId, RequestFilters filters, CancellationToken cancellationToken = default);
		Task<Result> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken = default);
	}
}
