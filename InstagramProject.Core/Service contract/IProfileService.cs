using InstagramProject.Core.Abstractions;
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
        Task<Result<UpdateProfileReauestBack>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
        Task<OperationResult> DeleteAsync(CancellationToken cancellationToken = default);

        Task<Result<GetUserDetailsResponse>> GetUserDetailsAsync(string userName, CancellationToken cancellationToken = default);

        Task<Result> ToggleFollowerAndFollowing(string userName, CancellationToken cancellationToken = default);
        Task<Result> ToggleNotificationFollowing(string userName, CancellationToken cancellationToken = default);
        Task<Result<PrivacyResponse>> GetPrivacyAsync(string userName, CancellationToken cancellationToken = default);
        Task<Result<NotificationPrivacyResponse>> GetNotificationPrivacy(string userName, CancellationToken cancellationToken = default);
    }
}
