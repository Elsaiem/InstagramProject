using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Common;
using InstagramProject.Core.Contracts.Home;
using InstagramProject.Core.Contracts.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
	public interface IHomeService
	{
		Task<PaginatedList<FeedResponse>> UserFeedAsync(string userId, RequestFilters request, CancellationToken cancellationToken = default);
		Task<Result<IEnumerable<SearchResponse>>> SearchForUserAsync(SearchRequest request, CancellationToken cancellationToken);
		Task<Result<IEnumerable<SuggestionsFollowerResponse>>> SuggestionsFollowerForYouAsync(string userId, CancellationToken cancellationToken);
	}
}
