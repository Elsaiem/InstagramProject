using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Reaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.ServiceContract
{
	public interface IReactionService
	{
		Task<Result<CreateReactionResponse>> CreateReactionAsync(string userId, CreateReactionRequest request, CancellationToken cancellationToken = default);
	}
}
