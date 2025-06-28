using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Authentication;

namespace InstagramProject.Core.Service_contract
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
		Task<Result> ResendConfirmEmailAsync(ResendConfirmationEmailRequest request);
        Task<Result> SendResetPasswordCodeAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
