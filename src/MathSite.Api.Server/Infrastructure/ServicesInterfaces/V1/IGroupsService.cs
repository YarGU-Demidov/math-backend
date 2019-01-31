using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IGroupsService : ICrudService<GroupDto>, IAliasedService<GroupDto>, IPageableService<GroupDto>
    {
        Task<ApiResponse<IEnumerable<GroupDto>>> GetAllAsync();
        Task<ApiResponse<bool>> HasRightAsync(Guid groupId, string rightAlias);
        Task<ApiResponse<IEnumerable<GroupDto>>> GetGroupsByTypeAsync(string groupTypeAlias);
    }
}