using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface IFileService: ICrudService<FileDto>
    {
        Task<ApiResponse<IEnumerable<FileDto>>> GetByDirectoryId(Guid directoryId);
    }
}
