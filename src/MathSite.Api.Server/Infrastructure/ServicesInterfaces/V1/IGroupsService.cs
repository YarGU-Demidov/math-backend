using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IGroupsService : ICrudService<GroupDto>, IAliasedService<GroupDto>, IPageableService<GroupDto>
    {
        [HttpGet(MethodNames.Groups.HasRight)]
        Task<ApiResponse<bool>> HasRightAsync(Guid groupId, string rightAlias);

        [HttpGet(MethodNames.Groups.GetGroupsByType)]
        Task<ApiResponse<IEnumerable<GroupDto>>> GetGroupsByTypeAsync(string groupTypeAlias);
    }
}