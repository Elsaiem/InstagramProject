using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Errors.FileErrors
{
	public static class FileErrors
	{
		public static readonly Error FileEmpty = new("File.Empty", "File Is Empty", StatusCodes.Status400BadRequest);
		public static readonly Error FileInvalidType = new("File.UploadFailed", "Only Images Are Supported", StatusCodes.Status400BadRequest);
		public static readonly Error EmptyPublicId = new("Media.EmptyPublicId", "Public ID cannot be empty", StatusCodes.Status400BadRequest);
		public static readonly Error DeleteFailed = new("Media.DeleteFailed", "Failed to delete media from storage", StatusCodes.Status400BadRequest);
	}
}
