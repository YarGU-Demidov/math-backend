using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface IPostSettingsService: ICrudService<PostSettingDto>, IPageableService<PostSettingDto>
    {
        Task<ApiResponse<PostSettingDto>> GetByPostIdAsync(PostDto post);
    }
}
