using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Abstractions.Consts
{	public class FileSettings
	{
		public const int MaxFileSizeInMB = 30;
		public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
		public static readonly string[] BlockedSignatures = ["4D-5A", "2F-2A", "D0-CF"];
		public static readonly string[] AllowedImagesExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];
		public static readonly string[] AllowedVideoExtensions = [".mp4", ".avi", ".mov", ".wmv", ".webm", ".mkv"];
		public static readonly string[] AllowedImageTypes = ["image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp", "image/webp"];
		public static readonly string[] AllowedVideoTypes = ["video/mp4", "video/avi", "video/mov", "video/wmv", "video/webm", "video/mkv"];
	}
}
