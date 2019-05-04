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
    [DefaultApiRoute(ServiceNames.PostSeoSettings)]
    [ApiController]
    public class PostSeoSettingsController : EntityApiControllerBase<PostSeoSetting>, IPostSeoSettingsService
    {
        private const string ServiceName = ServiceNames.PostSeoSettings;

        private readonly CrudServiceMethods<PostSeoSetting, PostSeoSettingDto> _crudServiceMethods;
        private readonly PageableServiceMethods<PostSeoSetting, PostSeoSettingDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<PostSeoSetting> _countableServiceMethods;
        public PostSeoSettingsController(MathSiteDbContext context, MathServices services, IMapper mapper, CrudServiceMethods<PostSeoSetting, PostSeoSettingDto> crudServiceMethods, PageableServiceMethods<PostSeoSetting, PostSeoSettingDto> pageableServiceMethods, CountableServiceMethods<PostSeoSetting> countableServiceMethods) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<PostSeoSettingDto>> GetById(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync(PostSeoSettingDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync(PostSeoSettingDto viewModel)
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
        public Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids)
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
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PostSeoSettingDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<PostSeoSettingDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var postSeoSettings = await Repository
                    .Include(ps => ps.Post)
                    .Include(ps => ps.PostKeywords)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<PostSeoSettingDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<PostSeoSettingDto>)postSeoSettings;
            });
        }

        [HttpGet(MethodNames.PostSeoSettings.GetByPostId)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.PostSeoSettings.GetByPostId)]
        public Task<ApiResponse<PostSeoSettingDto>> GetByPostIdAsync([FromBody]PostDto post)
        {
            return ExecuteSafely(async () =>
            {
                var seoSetting = await Repository.Include(ps => ps.Post).FirstOrDefaultAsync(ps => ps.Post.Id == post.Id);

                return Mapper.Map<PostSeoSettingDto>(seoSetting);
            });
        }

        protected async Task<PostSeoSetting> ViewModelToEntityAsync(PostSeoSettingDto viewModel, ActionType actionType)
        {
            PostSeoSetting postSeoSetting;
            if (actionType == ActionType.Create)
            {
                postSeoSetting = new PostSeoSetting();
                Mapper.Map(viewModel, postSeoSetting);
            }
            else
            {
                postSeoSetting = await Repository.FirstOrDefaultAsync(c => c.Id == viewModel.Id);

                Mapper.Map(viewModel, postSeoSetting);
            }


            return postSeoSetting;
        }
    }
}
