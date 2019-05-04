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
    [DefaultApiRoute(ServiceNames.PostTypes)]
    [ApiController]
    public class PostTypesController : EntityApiControllerBase<PostType>, IPostTypesService
    {
        private const string ServiceName = ServiceNames.PostTypes;

        private readonly CrudServiceMethods<PostType, PostTypeDto> _crudServiceMethods;
        private readonly PageableServiceMethods<PostType, PostTypeDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<PostType> _countableServiceMethods;

        public PostTypesController(MathSiteDbContext context, MathServices services, IMapper mapper, CrudServiceMethods<PostType, PostTypeDto> crudServiceMethods, PageableServiceMethods<PostType, PostTypeDto> pageableServiceMethods, CountableServiceMethods<PostType> countableServiceMethods) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<PostTypeDto>> GetById(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]PostTypeDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody]PostTypeDto viewModel)
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
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PostTypeDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<PostTypeDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var postType = await Repository
                    .Include(pt=>pt.DefaultPostsSettings)
                    .Include(pt => pt.Posts)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<PostTypeDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<PostTypeDto>)postType;
            });
        }

        [HttpGet(MethodNames.Global.GetByAlias)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetByAlias)]
        public Task<ApiResponse<IEnumerable<PostTypeDto>>> GetByAliasAsync(string alias)
        {
            return ExecuteSafely(async () =>
            {
                var postTypes = await Repository
                    .Where(pt => pt.Alias.ToLower().Contains(alias.ToLower()))
                    .Select(pt => Mapper.Map<PostTypeDto>(pt))
                    .ToArrayAsync();
                return (IEnumerable<PostTypeDto>)postTypes;
            });
        }

        [HttpGet(MethodNames.PostTypes.GetByPostId)]
        [AuthorizeMethod(ServiceName, MethodNames.PostTypes.GetByPostId)]
        public Task<ApiResponse<PostTypeDto>> GetByPostIdAsync([FromBody]PostDto post)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(post.PostTypeId));
        }

        protected async Task<PostType> ViewModelToEntityAsync(PostTypeDto viewModel, ActionType actionType)
        {
            PostType postType;
            if (actionType == ActionType.Create)
            {
                postType = new PostType();
                Mapper.Map(viewModel, postType);
            }
            else
            {
                postType = await Repository.FirstOrDefaultAsync(c => c.Id == viewModel.Id);

                Mapper.Map(viewModel, postType);
            }


            return postType;
        }
    }
}

