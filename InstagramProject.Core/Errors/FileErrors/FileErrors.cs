using InstagramProject.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Errors.FileErrors
{
	public static class FilesErrors
	{
		public static class FileErrors
		{
			public static readonly Error FileEmpty = new ("File.Empty", "File Is Empty", StatusCodes.Status400BadRequest);
			public static readonly Error FileInvalidType = new ("File.UploadFailed", "Only Images Are Supported", StatusCodes.Status400BadRequest);
		}
	}
}
