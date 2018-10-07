using System;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class CountableApiControllerBase<TEntity> : CountableApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        protected CountableApiControllerBase(MathSiteDbContext context, MathServices services) : base(context, services)
        {
        }
    }

    public abstract class CountableApiControllerBase<TEntity, TPrimaryKey> : ApiControllerBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        protected CountableApiControllerBase(MathSiteDbContext context, MathServices services) : base(context, services)
        {
        }

        [HttpPost(MethodNames.Global.GetCount)]
        public virtual async Task<ApiResponse<int>> GetCountAsync()
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.GetCount, async () => await Repository.CountAsync());
        }
    }
}