using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class CrudPageableWithAliasApiControllerBase<TViewModel, TEntity, TPrimaryKey> : CrudPageableApiControllerBase<TViewModel, TEntity, TPrimaryKey>
        where TViewModel : BaseEntityWithAlias<TPrimaryKey>
        where TEntity : class, IEntityWithAlias<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        protected CrudPageableWithAliasApiControllerBase(
            MathSiteDbContext context, 
            MathServices services, 
            IMapper mapper
        ) : base(context, services, mapper)
        {
        }

        [HttpGet(MethodNames.Global.GetByAlias)]
        public async Task<ApiResponse<TViewModel>> GetByAliasAsync(string alias)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.GetByAlias, async () =>
            {
                var entity = await Repository.Where(e => e.Alias == alias).FirstAsync();
                var model = Mapper.Map<TEntity, TViewModel>(entity);
                return model;
            });
        }
    }
}