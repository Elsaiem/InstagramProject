using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Abstractions.Consts;
using InstagramProject.Core.Contracts.Files;
using InstagramProject.Core.Helpers;
using InstagramProject.Core.Service_contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static InstagramProject.Core.Errors.FileErrors.FilesErrors;
using AppError = InstagramProject.Core.Abstractions.Error;

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
			{
				var cloudinaryError = result.Error;
				return Result.Failure<FileUploadResponse>(new AppError(cloudinaryError.Message, cloudinaryError.Message, StatusCodes.Status400BadRequest));
			}

			var response = new FileUploadResponse(
				result.Url.ToString(),
				result.PublicId,
				result.SecureUrl.ToString(),
				result.Format
			);
			return Result.Success(response);
		}
		public static bool IsVideoFile(IFormFile file)
		{
			return FileSettings.AllowedVideoTypes.Contains(file.ContentType.ToLower());
		}
	}
}