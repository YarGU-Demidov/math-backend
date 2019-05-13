using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface IDirectoryService: ICrudService<DirectoryDto>
    {
        Task<ApiResponse<IEnumerable<DirectoryDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<DirectoryDto>>> GetRootDirectories();
        Task<ApiResponse<IEnumerable<DirectoryDto>>> GetChildDirectories(Guid parentId);
    }
}
