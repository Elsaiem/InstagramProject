using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
	public interface IHomeService
	{
		Task<Result<IEnumerable<FeedResponse>>> UserFeedAsync(string userId, CancellationToken cancellationToken);
	}
}
