using System;
using System.Linq;
using System.Linq.Expressions;
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
    public abstract class CrudPageableWithAliasApiControllerBase<TViewModel, TEntity> : CrudPageableWithAliasApiControllerBase<TViewModel, TEntity, Guid>
        where TViewModel : BaseEntityWithAlias
        where TEntity : EntityWithAlias<Guid>
    {
        protected CrudPageableWithAliasApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper) 
            : base(context, services, mapper)
        {
        }

        protected override Expression<Func<TEntity, bool>> GetIdComparer(Guid compareToKey)
        {
            return entity => entity.Id == compareToKey;
        }
    }

    public abstract class CrudPageableWithAliasApiControllerBase<TViewModel, TEntity, TPrimaryKey> : CrudPageableApiControllerBase<TViewModel, TEntity, TPrimaryKey>
        where TViewModel : BaseEntityWithAlias<TPrimaryKey>
        where TEntity : EntityWithAlias<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        protected CrudPageableWithAliasApiControllerBase(
            MathSiteDbContext context, 
            MathServices services, 
            IMapper mapper
        ) : base(context, services, mapper)
        {
        }

        protected abstract bool GetAliasWithAccessCheck { get; }

        [HttpGet(MethodNames.Global.GetByAlias)]
        public virtual async Task<ApiResponse<TViewModel>> GetByAliasAsync(string alias)
        {
            async Task<TViewModel> GetByAliasPredicate()
            {
                var entity = await Repository.Where(e => e.Alias == alias).FirstAsync();
                var model = Mapper.Map<TEntity, TViewModel>(entity);
                return model;
            }

            return GetAliasWithAccessCheck 
                ? await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.GetByAlias, GetByAliasPredicate)
                : await ExecuteSafely(GetByAliasPredicate);
        }
    }
}