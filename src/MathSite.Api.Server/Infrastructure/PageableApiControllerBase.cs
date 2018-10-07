using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class PageableApiControllerBase<TEntity> : PageableApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        protected PageableApiControllerBase(MathSiteDbContext context, MathServices services) : base(context, services)
        {
        }
    }

    public abstract class PageableApiControllerBase<TEntity, TPrimaryKey> : CountableApiControllerBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        protected PageableApiControllerBase(MathSiteDbContext context, MathServices services) : base(context, services)
        {
        }

        [HttpPost(MethodNames.Global.GetPaged)]
        public virtual async Task<ApiResponse<IEnumerable<TEntity>>> GetAllPagedAsync(int page, int perPage)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.GetPaged, async () =>
            {
                page = page >= 1 ? page : 1;
                perPage = perPage > 0 ? perPage : 1;

                var skip = (page - 1) * perPage;

                return (IEnumerable<TEntity>) await Repository.PageBy(perPage, skip).ToArrayAsync();
            });
        }
    }
}