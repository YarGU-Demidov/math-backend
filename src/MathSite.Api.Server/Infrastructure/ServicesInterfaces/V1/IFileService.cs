using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface IFileService
    {
        Task<ApiResponse<IEnumerable<FileDto>>> GetByDirectoryId(Guid directoryId);
      //  Task<ApiResponse<IEnumerable<Guid>>> UploadFile(List<IFormFile> file);
    }
}
