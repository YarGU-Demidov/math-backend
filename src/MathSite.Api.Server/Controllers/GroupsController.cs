using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Groups)]
    [ApiController]
    public class GroupsController : EntityApiControllerBase<Group>, IGroupsService
    {
        private const string ServiceName = ServiceNames.Groups;

        private readonly CrudServiceMethods<Group, GroupDto> _crudServiceMethods;
        private readonly PageableServiceMethods<Group, GroupDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<Group> _countableServiceMethods;

        public GroupsController(
            MathSiteDbContext context, 
            MathServices services, 
            IMapper mapper,
            CrudServiceMethods<Group, GroupDto> crudServiceMethods,
            PageableServiceMethods<Group, GroupDto> pageableServiceMethods,
            CountableServiceMethods<Group> countableServiceMethods
        ) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<GroupDto>> GetById(Guid id)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync(GroupDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.CreateAsync(viewModel));
        }

        [HttpPost(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync(GroupDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.UpdateAsync(viewModel));
        }

        [HttpPost(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpPost(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<GroupDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpPost(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpGet(MethodNames.Groups.HasRight)]
        public async Task<ApiResponse<bool>> HasRightAsync(Guid groupId, string rightAlias)
        {
            async Task<bool> HasRight()
            {
                var selectedGroup = await Repository
                    .Include(group => group.GroupsRights)
                    .ThenInclude(right => right.Right)
                    .FirstOrDefaultAsync(group => group.Id == groupId);

                if (selectedGroup == null)
                    return false;

                if (selectedGroup.IsAdmin)
                    return true;

                if (selectedGroup.GroupsRights.Count == 0)
                    return false;

                return selectedGroup.GroupsRights
                    .Where(right => right.Right.Alias == rightAlias)
                    .All(right => right.Allowed);
            }

            return await ExecuteSafely(HasRight);
        }

        [HttpGet(MethodNames.Groups.GetGroupsByType)]
        [AuthorizeMethod(ServiceName, MethodNames.Groups.GetGroupsByType)]
        public Task<ApiResponse<IEnumerable<GroupDto>>> GetGroupsByTypeAsync(string groupTypeAlias)
        {
            throw new NotImplementedException();
        }

        [AuthorizeMethod(ServiceName, MethodNames.Global.GetByAlias)]
        public Task<ApiResponse<GroupDto>> GetByAliasAsync(string alias)
        {
            throw new NotImplementedException();
        }
    }
}