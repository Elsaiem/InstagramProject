using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Contracts.Profile;
using InstagramProject.Core.Entities;
using InstagramProject.Core.Errors.ProfileError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
	public interface IProfileService
	{
        Task<Result<UpdateProfileRequestBack>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult> DeleteAsync(CancellationToken cancellationToken = default);

        // Task<Result<GetUserDetailsResponse>> GetUserDetailsAsync(string userName, CancellationToken cancellationToken = default);
        Task<Result> HandleFollowActionAsync(string targetUserId, CancellationToken cancellationToken = default);
        Task<Result<FollowStatus>> GetFollowStatusAsync(string targetUserId, CancellationToken cancellationToken = default);

        // Remove these as they'll be replaced by HandleFollowActionAsync
        // Task<OperationResult> AddUserFollowAsync(AddFollowRequest response, CancellationToken cancellationToken = default);
         Task<OperationResult> DeleteUserFollowAsync(AddFollowRequest response, CancellationToken cancellationToken = default);

        // Keep other existing methods
        Task<Result<IEnumerable<UserDataFollow>>> GetAllFollowers(string userName, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserDataFollow>>> GetAllFollowing(string userName, CancellationToken cancellationToken = default);
        Task<Result<FollowerDetailsResponse>> GetFollowersDetailsAsync(string userId, string followName, CancellationToken cancellationToken = default);
        Task<Result> RemoveFollowersAsync(RemoveFollowerRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleFollowerAndFollowing(string userName, CancellationToken cancellationToken = default);
        Task<Result> TogglePrivacyTheAccount(string userName, CancellationToken cancellationToken = default);

        Task<Result<PrivacyResponse>> GetPrivacyAsync(string userName, CancellationToken cancellationToken = default);
       // Task<Result> SendFollowRequestAsync(SendFollowRequestRequest request, CancellationToken cancellationToken = default);
        Task<Result> AcceptFollowRequestAsync(string requestId, CancellationToken cancellationToken = default);
        Task<Result> RejectFollowRequestAsync(string requestId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<FollowRequest>>> GetPendingFollowRequestsAsync(CancellationToken cancellationToken = default);
        //Task<Result<NotificationPrivacyResponse>> GetNotificationPrivacy(string userName, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<PostResponse>>> GetUserPosts(string userId, CancellationToken cancellationToken = default);

    }
}
