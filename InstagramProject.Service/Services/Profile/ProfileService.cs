using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Comment;
using InstagramProject.Core.Contracts.Post;
using InstagramProject.Core.Contracts.Profile;
using InstagramProject.Core.Entities;
using InstagramProject.Core.Entities.Auth;
using InstagramProject.Core.Errors.Authentication;
using InstagramProject.Core.Errors.Post;
using InstagramProject.Core.Errors.ProfileError;
using InstagramProject.Core.Repository_Contract;
using InstagramProject.Core.Service_contract;
using InstagramProject.Repository.Data.Contexts;
using InstagramProject.Service.Services.Files;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Security.Claims;
using System.Text.Json;
using static InstagramProject.Core.Errors.Authentication.UserErrors;
namespace InstagramProject.Service.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;

        public ProfileService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IFileService fileService, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IUnitOfWork unitOfWork, HttpClient httpClient, IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result> AcceptFollowRequestAsync(string requesterId, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(UserErrors.Unauthorized);

            var followRequest = await _context.FollowRequests
                .FirstOrDefaultAsync(fr => fr.RequesterId == requesterId && fr.TargetUserId == userId && !fr.IsAccepted);
            if (followRequest == null)
                return Result.Failure(UserErrors.FollowRequestNotFound);

            followRequest.IsAccepted = true;
            var follow = new UserFollow
            {
                UserId = requesterId,
                FollowId = userId,
                FollowedOn = DateTime.UtcNow
            };

                  _context.FollowRequests.Remove(followRequest);
            await _context.UserFollows.AddAsync(follow);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        

        public async Task<OperationResult> DeleteAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return OperationResult.Failure("User is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return OperationResult.Failure("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return OperationResult.Failure("Failed to delete user account.");

            await _signInManager.SignOutAsync();

            return OperationResult.Success("User deleted and signed out.");
        }

        public async Task<OperationResult> DeleteUserFollowAsync(AddFollowRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.FollowId))
                    return OperationResult.Failure("Invalid unfollow request.");

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return OperationResult.Failure("Unauthorized user.");

                var userRepo = _unitOfWork.Repository<ApplicationUser>().GetQueryable();
                var userToUnfollow = await userRepo.FirstOrDefaultAsync(u => u.Id == request.FollowId, cancellationToken);

                if (userToUnfollow == null)
                    return OperationResult.Failure($"User '{request.FollowId}' not found.");

                var followEntity = await _unitOfWork.Repository<UserFollow>()
                    .GetQueryable()
                    .Where(uf => uf.UserId == userId && uf.FollowId == userToUnfollow.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (followEntity == null)
                    return OperationResult.Failure("Follow relationship not found.");

                _unitOfWork.Repository<UserFollow>().Delete(followEntity);
                await _unitOfWork.CompleteAsync();

                return OperationResult.Success("Unfollowed successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Failed to unfollow user: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<UserDataFollow>>> GetAllFollowers(string userName, CancellationToken cancellationToken = default)
        {
            var userIdToken = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdToken))
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.Unauthorized);

            var userRepo = _unitOfWork.Repository<ApplicationUser>().GetQueryable();
            var user = await userRepo.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

            if (user == null)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.UserNotFound);

            bool isOwnProfile = userIdToken == user.Id;
            if (!isOwnProfile && user.IsEnableFollowerAndFollowing)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.FollowersHidden);
            var userFollowRepo = _unitOfWork.Repository<UserFollow>().GetQueryable();
            var followers = await (from follow in userFollowRepo
                                   join followerUser in userRepo on follow.UserId equals followerUser.Id
                                   where follow.FollowId == user.Id
                                   select new UserDataFollow
                                   {
                                       UserId = followerUser.UserName!,
                                       FullName = followerUser.FullName,
                                       ProfilePic = followerUser.ProfilePic,
                                       followedOn = follow.FollowedOn,
                                       IsFollow = userFollowRepo.Any(uf => uf.UserId == userIdToken && uf.FollowId == followerUser.Id)
                                   }).ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<UserDataFollow>>(followers);
        }

        public async Task<Result<IEnumerable<UserDataFollow>>> GetAllFollowing(string userName, CancellationToken cancellationToken = default)
        {
            var userIdToken = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdToken))
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.Unauthorized);

            var userRepo = _unitOfWork.Repository<ApplicationUser>().GetQueryable();
            var user = await userRepo.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
            if (user == null)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.UserNotFound);

            bool isOwnProfile = userIdToken == user.Id;
            if (!isOwnProfile && user.IsEnableFollowerAndFollowing)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.FollowingHidden);

            var userFollowRepo = _unitOfWork.Repository<UserFollow>().GetQueryable();
            var following = await (from follow in userFollowRepo
                                   join followedUser in userRepo on follow.FollowId equals followedUser.Id
                                   where follow.UserId == user.Id
                                   select new UserDataFollow
                                   {
                                       UserId = followedUser.UserName!,
                                       FullName = followedUser.FullName,
                                       ProfilePic = followedUser.ProfilePic,
                                       followedOn = follow.FollowedOn,
                                       IsFollow = userIdToken != null && userFollowRepo.Any(uf => uf.UserId == userIdToken && uf.FollowId == followedUser.Id)
                                   }).ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<UserDataFollow>>(following);
        }

        public async Task<Result<FollowerDetailsResponse>> GetFollowersDetailsAsync(string userId, string followName, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<ApplicationUser>().GetQueryable();
            var user = await userRepo.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user is null)
                return Result.Failure<FollowerDetailsResponse>(UserErrors.UserNotFound);

            var follower = await userRepo.FirstOrDefaultAsync(u => u.UserName == followName, cancellationToken);
            if (follower is null)
                return Result.Failure<FollowerDetailsResponse>(UserErrors.FollowerNotFound);

            var userFollowRepo = _unitOfWork.Repository<UserFollow>().GetQueryable();
            var isFollowing = await userFollowRepo.AnyAsync(uf => uf.UserId == userId && uf.FollowId == follower.Id, cancellationToken);
            var response = new FollowerDetailsResponse(
                follower.UserName!,
                follower.FullName,
                follower.ProfilePic,
                isFollowing
               
            );
            return Result.Success(response);
        }

        public async Task<Result<FollowStatus>> GetFollowStatusAsync(string targetUserId, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure<FollowStatus>(UserErrors.Unauthorized);

            var isFollowing = await _context.UserFollows
                .AnyAsync(uf => uf.UserId == userId && uf.FollowId == targetUserId);
            if (isFollowing)
                return Result.Success(FollowStatus.Following);

            var hasPendingRequest = await _context.FollowRequests
                .AnyAsync(fr => fr.RequesterId == userId && fr.TargetUserId == targetUserId && !fr.IsAccepted);
            if (hasPendingRequest)
                return Result.Success(FollowStatus.Requested);

            return Result.Success(FollowStatus.NotFollowing);
        }

        public async Task<Result<IEnumerable<FollowRequest>>> GetPendingFollowRequestsAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure<IEnumerable<FollowRequest>>(UserErrors.Unauthorized);

            var pendingRequests = await _context.FollowRequests
                .Where(fr => fr.TargetUserId == userId && !fr.IsAccepted)
                .Include(fr => fr.Requester)
                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<FollowRequest>>(pendingRequests);
        }

        public async Task<Result<PrivacyResponse>> GetPrivacyAsync(string userName, CancellationToken cancellationToken = default)
        {
            if (userName is null)
                return Result.Failure<PrivacyResponse>(UserErrors.UserNameNotFound);
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return Result.Failure<PrivacyResponse>(UserErrors.UserNameNotFound);

            var privacyResponse = new PrivacyResponse
            (
                IsEnableFollowerAndFollowing: user.IsEnableFollowerAndFollowing,
                IsEnablePublicOrPrivate: user.IsEnablePublicOrPrivate
            );
            return Result.Success(privacyResponse);
        }

        public async Task<Result> HandleFollowActionAsync(string targetUserId, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(UserErrors.Unauthorized);

            if (userId == targetUserId)
                return Result.Failure(UserErrors.CannotFollowYourself);

            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
                return Result.Failure(UserErrors.UserNotFound);

            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.FollowId == targetUserId);
            if (existingFollow != null)
                return Result.Failure(UserErrors.AlreadyFollowing);

            var existingRequest = await _context.FollowRequests
                .FirstOrDefaultAsync(fr => fr.RequesterId == userId && fr.TargetUserId == targetUserId);
            if (existingRequest != null)
                return Result.Failure(UserErrors.FollowRequestAlreadyExists);

            if (targetUser.IsEnablePublicOrPrivate)
            {
                // Create follow request for private account
                var followRequest = new FollowRequest
                {
                    RequesterId = userId,
                    TargetUserId = targetUserId,
                    RequestedAt = DateTime.UtcNow
                };
                await _context.FollowRequests.AddAsync(followRequest);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Follow request has been sent successfully.");
            }
            else
            {
                // Direct follow for public account
                var follow = new UserFollow
                {
                    UserId = userId,
                    FollowId = targetUserId,
                    FollowedOn = DateTime.UtcNow
                };
                await _context.UserFollows.AddAsync(follow);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("You are now following the user.");
            }
        }

        public async Task<Result> RejectFollowRequestAsync(string requesterId, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(UserErrors.Unauthorized);

            var followRequest = await _context.FollowRequests
                .FirstOrDefaultAsync(fr => fr.RequesterId == requesterId && fr.TargetUserId == userId && !fr.IsAccepted);
            if (followRequest == null)
                return Result.Failure(UserErrors.FollowRequestNotFound);

            _context.FollowRequests.Remove(followRequest);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> RemoveFollowersAsync( RemoveFollowerRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure<UpdateProfileRequestBack>(UserErrors.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.Unauthorized);

            var follower = await _userManager.FindByNameAsync(request.FollowerUserName);
            if (follower == null)
                return Result.Failure<IEnumerable<UserDataFollow>>(UserErrors.UserNotFound);

            var followRelation = await _context.UserFollows.FirstOrDefaultAsync(uf => uf.UserId == follower.Id && uf.FollowId == user.Id, cancellationToken);
            if (followRelation == null)
                return Result.Failure(UserErrors.FollowNotFound);

            _context.UserFollows.Remove(followRelation);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result<UpdateProfileRequestBack>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure<UpdateProfileRequestBack>(UserErrors.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<UpdateProfileRequestBack>(UserErrors.UserNotFound); // Changed from DublicatedUserName

            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.UserName) && request.UserName != user.UserName)
            {
                var userNameExists = await _userManager.Users.AnyAsync(x => x.UserName == request.UserName, cancellationToken);
                if (userNameExists)
                    return Result.Failure<UpdateProfileRequestBack>(UserErrors.DublicatedUserName);

                var userNameResult = await _userManager.SetUserNameAsync(user, request.UserName);
                if (!userNameResult.Succeeded)
                {
                    var error = userNameResult.Errors.First();
                    return Result.Failure<UpdateProfileRequestBack>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                if (request.Email.Contains("@gmail.com") || request.Email.Contains("@yahoo.com") || request.Email.Contains("@fayoum.edu.eg"))
                {
                    var emailResult = await _userManager.SetEmailAsync(user, request.Email);
                    if (!emailResult.Succeeded)
                    {
                        var error = emailResult.Errors.First();
                        return Result.Failure<UpdateProfileRequestBack>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
                    }
                }
                else
                {
                    return Result.Failure<UpdateProfileRequestBack>(UserErrors.InvalidEmailDomain);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Bio))
                user.Bio = request.Bio;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
                if (!passwordResult.Succeeded)
                {
                    var error = passwordResult.Errors.First();
                    return Result.Failure<UpdateProfileRequestBack>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
                }
            }

            if (request.Profile_Image != null && request.Profile_Image.Any())
            {
                // Delete old images
                if (!string.IsNullOrEmpty(user.ProfilePic))
                {
                    var mediaObjects = JsonSerializer.Deserialize<List<JsonElement>>(user.ProfilePic);
                    if (mediaObjects != null)
                    {
                        foreach (var mediaObj in mediaObjects)
                        {
                            var url = mediaObj.GetProperty("url").GetString();
                            if (!string.IsNullOrEmpty(url))
                            {
                                var publicId = CloudinaryService.ExtractPublicIdFromUrl(url);
                                if (!string.IsNullOrEmpty(publicId))
                                {
                                    var deleteResult = await _fileService.DeleteFromCloudinaryAsync(publicId);
                                    if (!deleteResult.IsSuccess)
                                        return Result.Failure<UpdateProfileRequestBack>(PostErrors.MediaDeletionFailed);
                                    ;
                                }
                            }
                        }
                    }
                }

                var profileMedia = new List<ProfileMedia>();
                foreach (var mediaFile in request.Profile_Image)
                {
                    var result = await _fileService.UploadToCloudinaryAsync(mediaFile);
                    if (result.IsSuccess)
                    {
                        profileMedia.Add(new ProfileMedia(result.Value.SecureUrl));
                    }
                }

                var mediaUrls = profileMedia.Select(media => new { url = media.MediaUrl }).ToList();
                user.ProfilePic = JsonSerializer.Serialize(mediaUrls);
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var error = updateResult.Errors.First();
                return Result.Failure<UpdateProfileRequestBack>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
            }

            var updated = new UpdateProfileRequestBack
            {
                UserName = user.UserName,
                Email = user.Email,
                Bio = user.Bio,
                Profile_Image = user.ProfilePic,
                FullName = user.FullName,
            };

            return Result.Success(updated);
        }
        public async Task<Result> ToggleFollowerAndFollowing(string userName, CancellationToken cancellationToken = default)
        {
            if (userName is null)
                return Result.Failure<PrivacyResponse>(UserErrors.UserNameNotFound);
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return Result.Failure(UserErrors.UserNameNotFound);
            user.IsEnableFollowerAndFollowing = !user.IsEnableFollowerAndFollowing;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> TogglePrivacyTheAccount(string userName, CancellationToken cancellationToken = default)
        {
            if (userName is null)
                return Result.Failure<PrivacyResponse>(UserErrors.UserNameNotFound);
            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return Result.Failure(UserErrors.UserNameNotFound);
            user.IsEnablePublicOrPrivate = !user.IsEnablePublicOrPrivate;
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<IEnumerable<PostResponse>>> GetUserPosts(string userId, CancellationToken cancellationToken = default)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return Result.Failure<IEnumerable<PostResponse>>(UserErrors.Unauthorized);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure<IEnumerable<PostResponse>>(UserErrors.UserNotFound);

            var isOwner = currentUserId == userId;
            var isFollowing = await _context.UserFollows
                .AnyAsync(uf => uf.UserId == currentUserId && uf.FollowId == userId, cancellationToken);

            if (!user.IsEnablePublicOrPrivate || isOwner || isFollowing)
            {
                var posts = await _context.posts
                    .AsNoTracking()
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.Time)
                    .ToListAsync(cancellationToken);

                var response = posts.Select(post =>
                {
                    var media = new List<PostMedia>();
                    if (!string.IsNullOrEmpty(post.PostMedia))
                    {
                        try
                        {
                            var mediaObjects = JsonSerializer.Deserialize<List<PostMedia>>(post.PostMedia);
                            if (mediaObjects != null)
                            {
                                media = mediaObjects
                                    .Where(m => !string.IsNullOrEmpty(m.MediaUrl) && !string.IsNullOrEmpty(m.MediaType))
                                    .Select(m => new PostMedia(m.MediaUrl, m.MediaType))
                                    .ToList();
                            }
                        }
                        catch
                        {
                            // Optionally handle invalid JSON
                        }
                    }

                    return new PostResponse(
                        post.Id,
                        post.UserId,
                        post.Content,
                        media
                    );
                }).ToList();

                return Result.Success<IEnumerable<PostResponse>>(response);
            }

            return Result.Failure<IEnumerable<PostResponse>>(UserErrors.PrivateAccountPostsNotVisible);
        }

    }
}
