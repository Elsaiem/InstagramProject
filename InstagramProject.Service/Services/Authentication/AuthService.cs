﻿using Hangfire;
using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Authentication_Contract;
using InstagramProject.Core.Contracts.Authentication;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Helpers;
using InstagramProject.Core.Service_contract;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using static InstagramProject.Core.Errors.Authentication.AuthenticationError;

namespace InstagramProject.Service.Services.Authentication
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IJwtProvider _jwtProvider;
		private readonly IEmailSender _emailSender;
		private readonly ILogger<AuthService> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly int _refreshTokenExpiryDays = 7;
		public AuthService(UserManager<ApplicationUser> userManager,
			   IJwtProvider jwtProvider,
			   SignInManager<ApplicationUser> signInManager,
			   ILogger<AuthService> logger,
			   IEmailSender emailSender,
			  IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_jwtProvider = jwtProvider;
			_signInManager = signInManager;
			_logger = logger;
			_httpContextAccessor = httpContextAccessor;
			_emailSender = emailSender;
		}
		public async Task<Result<AuthResponse>> GetTokenAsync(LoginRequest request, CancellationToken cancellationToken = default)
		{
			var user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
			if (user is null)
			{
				user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
				if (user is null)
					return Result.Failure<AuthResponse>(UserErrors.UserNotFound);
			}
			var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
			if (result.Succeeded)
			{
				var userRole = await GetUserRole(user, cancellationToken);
				var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRole);

				var refreshToken = GenerateRefreshToken();
				var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

				user.RefreshTokens.Add(new RefreshToken
				{
					Token = refreshToken,
					ExpiresOn = refreshTokenExpiration
				});

				await _userManager.UpdateAsync(user);

				var response = new AuthResponse(user.Id, user.Email, user.UserName!, token, expiresIn, refreshToken, user.ProfilePic, refreshTokenExpiration);

				return Result.Success(response);
			}

			return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentails);
		}
		public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);

			if (userId is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var user = await _userManager.FindByIdAsync(userId);

			if (user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			if (user.IsDisabled)
				return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

			if (userRefreshToken is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

			userRefreshToken.RevokedOn = DateTime.UtcNow;

			var userRoles = await GetUserRole(user, cancellationToken);

			var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);
			var newRefreshToken = GenerateRefreshToken();
			var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

			user.RefreshTokens.Add(new RefreshToken
			{
				Token = newRefreshToken,
				ExpiresOn = refreshTokenExpiration
			});

			await _userManager.UpdateAsync(user);

			var response = new AuthResponse(user.Id, user.Email, user.UserName!, newToken, expiresIn, newRefreshToken, user.ProfilePic, refreshTokenExpiration);

			return Result.Success(response);
		}
		public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
		{
			var userId = _jwtProvider.ValidateToken(token);
			if (userId is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
			if (userRefreshToken is null)
				return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

			userRefreshToken.RevokedOn = DateTime.UtcNow;
			await _userManager.UpdateAsync(user);

			return Result.Success();
		}
		public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
		{
			var emailIsExist = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
			if (emailIsExist)
				return Result.Failure(UserErrors.DublicatedEmail);

			var userNameIsExist = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);
			if (userNameIsExist)
				return Result.Failure(UserErrors.DublicatedUserName);

			var user = request.Adapt<ApplicationUser>();
			var result = await _userManager.CreateAsync(user, request.Password);
			if (result.Succeeded)
			{
				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				_logger.LogInformation("Confirmation Code {code}", code);

				await SendConfirmationEmailAsync(user, code);

				return Result.Success();
			}
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
		public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
		{
			var user = await _userManager.FindByIdAsync(request.UserId);
			if (user is null)
				return Result.Failure(UserErrors.InvalidCode);

			if (user.EmailConfirmed)
				return Result.Failure(UserErrors.DuplicatedConfirmation);

			var code = request.Code;
			try
			{
				code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			}
			catch (FormatException)
			{
				return Result.Failure(UserErrors.InvalidCode);
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
				return Result.Success();
			}
			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
		}
		public async Task<Result> SendResetPasswordCodeAsync(string email)
		{

			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return Result.Failure(UserErrors.UserEmailNotFound);

			if (!user.EmailConfirmed)
				return Result.Failure(UserErrors.EmailNotConfirmed);

			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			_logger.LogInformation("Reset Password Code {code}", code);

			await SendResetPasswordEmailAsync(user, code);

			return Result.Success();
		}
		public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null)
				return Result.Failure(UserErrors.UserEmailNotFound);

			if (!user.EmailConfirmed)
				return Result.Failure(UserErrors.EmailNotConfirmed);

			IdentityResult result;
			try
			{
				var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
				result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
			}
			catch (FormatException)
			{
				result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
			}

			if (result.Succeeded)
				return Result.Success();

			var error = result.Errors.First();
			return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
		}
		private static string GenerateRefreshToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}
		private async Task<string> GetUserRole(ApplicationUser user, CancellationToken cancellationToken)
		{
			var userRoles = await _userManager.GetRolesAsync(user);
			return userRoles.FirstOrDefault()!;
		}
		public async Task<Result> ResendConfirmEmailAsync(ResendConfirmationEmailRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user is null)
				return Result.Success();

			if (user.EmailConfirmed)
				return Result.Failure(UserErrors.DuplicatedConfirmation);

			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			_logger.LogInformation("Confirmation Code {code}", code);

			await SendConfirmationEmailAsync(user, code);

			return Result.Success();
		}

		private async Task SendConfirmationEmailAsync(ApplicationUser user, string code)
		{
			var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

			var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
				new Dictionary<string, string>
				{
					{"{{name}}",user.UserName },
					{ "{{action_url}}",$"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
				}
			);
			BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Instagram: Confirm your email", emailBody));

			await Task.CompletedTask;
		}

		private async Task SendResetPasswordEmailAsync(ApplicationUser user, string code)
		{
			var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

			var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
				new Dictionary<string, string>
				{
					{"{{name}}",user.UserName },
					{ "{{action_url}}",$"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
				}
			);
			BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "✅ Instagram: Reset your password", emailBody));

			await Task.CompletedTask;
		}
	}
}
