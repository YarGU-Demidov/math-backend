using System;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class CountableApiControllerBase<TEntity> : CountableApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        protected CountableApiControllerBase(MathSiteDbContext context) : base(context)
        {
        }
    }

    public abstract class CountableApiControllerBase<TEntity, TPrimaryKey> : ApiControllerBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {

        protected CountableApiControllerBase(MathSiteDbContext context) : base(context)
        {
        }

        [HttpPost(MethodNames.Global.GetCount)]
        public virtual async Task<ApiResponse> GetCountAsync()
        {
            try
            {
                return new DataApiResponse<int>(await Repository.CountAsync());
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }
    }
}