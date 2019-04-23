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
    [DefaultApiRoute(ServiceNames.Categories)]
    [ApiController]
    public class CategoriesController : EntityApiControllerBase<Category>, ICategoriesService
    {
        private const string ServiceName = ServiceNames.Groups;

        private readonly CrudServiceMethods<Category, CategoryDto> _crudServiceMethods;
        private readonly PageableServiceMethods<Category, CategoryDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<Category> _countableServiceMethods;

        public CategoriesController(
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            CrudServiceMethods<Category, CategoryDto> crudServiceMethods,
            PageableServiceMethods<Category, CategoryDto> pageableServiceMethods,
            CountableServiceMethods<Category> countableServiceMethods
        ) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]CategoryDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
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

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var category = await Repository
                    .Include(c=>c.PostCategories)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<CategoryDto>(u))
                    .ToArrayAsync();
                return (IEnumerable<CategoryDto>)category;
            });
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet(MethodNames.Global.GetByAlias)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetByAlias)]
        public Task<ApiResponse<IEnumerable<CategoryDto>>> GetByAliasAsync(string alias)
        {
            return ExecuteSafely(async () =>
            {
                var persons = await Repository
                    .Where(p => p.Alias.ToLower().Contains(alias.ToLower()))
                    .Select(p => Mapper.Map<CategoryDto>(p))
                    .ToArrayAsync();
                return (IEnumerable<CategoryDto>)persons;
            });
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<CategoryDto>> GetById(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(id));
        }

        [HttpGet(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody]CategoryDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }
        protected async Task<Category> ViewModelToEntityAsync(CategoryDto viewModel, ActionType actionType)
        {
            Category category;
            if (actionType == ActionType.Create)
            {
                category = new Category();
                Mapper.Map(viewModel, category);
            }
            else
            {
                category = await Repository.FirstOrDefaultAsync(c => c.Id == viewModel.Id);
                
                Mapper.Map(viewModel, category);
            }
            

            return category;
        }
    }
}