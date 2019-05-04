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
    [DefaultApiRoute(ServiceNames.PostSettings)]
    [ApiController]
    public class PostSettingsController: EntityApiControllerBase<PostSetting>, IPostSettingsService
    {
        private const string ServiceName = ServiceNames.PostSettings;

        private readonly CrudServiceMethods<PostSetting, PostSettingDto> _crudServiceMethods;
        private readonly PageableServiceMethods<PostSetting, PostSettingDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<PostSetting> _countableServiceMethods;
        public PostSettingsController(MathSiteDbContext context, MathServices services, IMapper mapper, CrudServiceMethods<PostSetting, PostSettingDto> crudServiceMethods, PageableServiceMethods<PostSetting, PostSettingDto> pageableServiceMethods, CountableServiceMethods<PostSetting> countableServiceMethods) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<PostSettingDto>> GetById(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]PostSettingDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody]PostSettingDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpDelete("delete-many")]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse<int>> DeleteManyAsync([FromBody]List<Guid> ids)
        {
            return ExecuteSafely(() =>
            {
                return Repository.Where(x => ids.Contains(x.Id)).DeleteFromQueryAsync();
            });
        }

        [HttpGet(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }


        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PostSettingDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<PostSettingDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var postSettings = await Repository
                    .Include(ps => ps.Post)
                    .Include(ps => ps.PostType)
                    .Include(ps => ps.PreviewImage)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<PostSettingDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<PostSettingDto>)postSettings;
            });
        }

        [HttpGet(MethodNames.PostSettings.GetByPostId)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.PostSettings.GetByPostId)]
        public Task<ApiResponse<PostSettingDto>> GetByPostIdAsync([FromBody]PostDto post)
        {
            return ExecuteSafely(async () =>
            {
                var setting = await Repository.Include(ps=>ps.Post).FirstOrDefaultAsync(ps=>ps.Post.Id == post.Id);

                return Mapper.Map<PostSettingDto>(setting);
            });
        }

        protected async Task<PostSetting> ViewModelToEntityAsync(PostSettingDto viewModel, ActionType actionType)
        {
            PostSetting postSetting;
            if (actionType == ActionType.Create)
            {
                postSetting = new PostSetting();
                Mapper.Map(viewModel, postSetting);
            }
            else
            {
                postSetting = await Repository.FirstOrDefaultAsync(c => c.Id == viewModel.Id);

                Mapper.Map(viewModel, postSetting);
            }


            return postSetting;
        }
    }
}
