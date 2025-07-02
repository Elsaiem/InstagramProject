using FluentValidation;
using InstagramProject.Core.Abstractions.Consts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Contracts.Files
{
	public class FileTypeValidator : AbstractValidator<IFormFile>
	{
		public FileTypeValidator()
		{
			RuleFor(x => x.ContentType)
				.Must(contentType =>
				{
					if (string.IsNullOrEmpty(contentType))
						return false;

					var normalizedContentType = contentType.ToLower();
					return FileSettings.AllowedImageTypes.Contains(normalizedContentType) ||
						   FileSettings.AllowedVideoTypes.Contains(normalizedContentType);
				})
				.WithMessage("Only image and video files are allowed")
				.When(x => x is not null);

			RuleFor(x => x.FileName)
				.Must(fileName =>
				{
					if (string.IsNullOrEmpty(fileName))
						return false;

					var extension = Path.GetExtension(fileName).ToLower();
					return FileSettings.AllowedImagesExtensions.Contains(extension) ||
						   FileSettings.AllowedVideoExtensions.Contains(extension);
				})
				.WithMessage("File extension is not allowed")
				.When(x => x is not null && !string.IsNullOrEmpty(x.FileName));
		}
	}
}
