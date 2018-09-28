using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class PageableApiControllerBase<TEntity> : PageableApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        protected PageableApiControllerBase(MathSiteDbContext context) : base(context)
        {
        }
    }

    public abstract class PageableApiControllerBase<TEntity, TPrimaryKey> : CountableApiControllerBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        protected PageableApiControllerBase(MathSiteDbContext context) : base(context)
        {
        }

        [HttpPost(MethodNames.Global.GetPaged)]
        public virtual async Task<ApiResponse> GetAllPagedAsync(int page, int perPage)
        {
            try
            {
                page = page >= 1 ? page : 1;
                perPage = perPage > 0 ? perPage : 1;

                var skip = (page - 1) * perPage;

                return new DataApiResponse<IEnumerable<TEntity>>(
                    data: await Repository.PageBy(perPage, skip).ToArrayAsync()
                );
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }
    }
}