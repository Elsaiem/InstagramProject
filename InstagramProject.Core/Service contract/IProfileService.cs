using InstagramProject.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
	public interface IProfileService
	{
		Task<Result> AddUserFollowAsync(string userId, string follwerId, CancellationToken cancellationToken = default);
		Task<Result> DeleteUserFollowAsync(string userId, string follwerId, CancellationToken cancellationToken = default);
	}
}
