using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IPostTypesService: ICrudService<PostTypeDto>, IPageableService<PostTypeDto>, IAliasedService<PostTypeDto>
    {
        Task<ApiResponse<PostTypeDto>> GetByPostIdAsync(PostDto post);
    }
}
