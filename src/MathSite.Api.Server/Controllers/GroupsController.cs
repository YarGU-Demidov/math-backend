using System;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.VersionsAttributes;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Groups)]
    [ApiController]
    public class GroupsController : CrudPageableWithAliasApiControllerBase<GroupDto, Group>
    {
        public GroupsController(MathSiteDbContext context, MathServices services, IMapper mapper) : base(context, services, mapper)
        {
        }

        [HttpGet(MethodNames.Groups.GetGroupsByType)]
        public async Task<ApiResponse<bool>> HasRightAsync(Guid groupId, string rightAlias)
        {
            return await ExecuteSafely(async () =>
            {
                return false;
            });
        }

        protected override string AreaName { get; } = ServiceNames.Groups;
    }
}