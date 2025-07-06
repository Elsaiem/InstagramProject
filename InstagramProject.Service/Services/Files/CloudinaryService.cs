using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Contracts.Files;
using InstagramProject.Core.Errors.FileErrors;
using InstagramProject.Core.Helpers;
using InstagramProject.Core.Service_contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InstagramProject.Service.Services.Files
{
	public class CloudinaryService : IFileService
	{
		private readonly Cloudinary _cloudinary;

		public CloudinaryService(IOptions<CloudinarySettings> config)
		{
			var account = new Account(
				config.Value.CloudName,
				config.Value.ApiKey,
				config.Value.ApiSecret
			);

			_cloudinary = new Cloudinary(account);
		}
		public async Task<Result<FileUploadResponse>> UploadToCloudinaryAsync(IFormFile file)
		{
			if (file.Length <= 0)
				return Result.Failure<FileUploadResponse>(FileErrors.FileEmpty);
			var isImage = FileSettings.AllowedImageTypes.Contains(file.ContentType.ToLower());
			using var stream = file.OpenReadStream();
			RawUploadParams uploadParams;
			if (isImage)
				uploadParams = new ImageUploadParams
				{
					File = new FileDescription(file.FileName, stream)
				};

			else
				uploadParams = new VideoUploadParams
				{
					File = new FileDescription(file.FileName, stream)
				};

			var result = await _cloudinary.UploadAsync(uploadParams);

			if (result.Error != null)
				return Result.Failure<FileUploadResponse>(new Core.Abstractions.Error(result.Error.Message, result.Error.Message, StatusCodes.Status400BadRequest));

			var response = new FileUploadResponse(
				result.Url.ToString(),
				result.PublicId,
				result.SecureUrl.ToString(),
				result.Format
			);
			return Result.Success(response);
		}
		public async Task<Result> DeleteFromCloudinaryAsync(string publicId)
		{
			if (string.IsNullOrEmpty(publicId))
				return Result.Failure(FileErrors.EmptyPublicId);

			var deleteParams = new DeletionParams(publicId);
			var result = await _cloudinary.DestroyAsync(deleteParams);

			if (result.Error != null)
				return Result.Failure(new Core.Abstractions.Error(result.Error.Message, result.Error.Message, StatusCodes.Status400BadRequest));

			if (result.StatusCode != System.Net.HttpStatusCode.OK)
				return Result.Failure(FileErrors.DeleteFailed);

			return Result.Success();
		}
		public static string ExtractPublicIdFromUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
				return string.Empty;

			var uri = new Uri(url);
			var pathSegments = uri.LocalPath.Split('/');
			var uploadIndex = Array.IndexOf(pathSegments, "upload");
			if (uploadIndex == -1)
				return string.Empty;

			var publicIdWithExtension = pathSegments[pathSegments.Length - 1];
			var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
			if (lastDotIndex > 0)
				return publicIdWithExtension.Substring(0, lastDotIndex);
			return publicIdWithExtension;
		}
		public static bool IsVideoFile(IFormFile file)
		{
			return FileSettings.AllowedVideoTypes.Contains(file.ContentType.ToLower());
		}
	}
}