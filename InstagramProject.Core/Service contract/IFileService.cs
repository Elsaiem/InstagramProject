using InstagramProject.Core.Abstractions;
using InstagramProject.Core.Contracts.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramProject.Core.Service_contract
{
    public interface IFileService
    {
        Task<Result<FileUploadResponse>> UploadToCloudinaryAsync(IFormFile file);
    }
}