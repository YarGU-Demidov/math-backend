using System;
using MathSite.Api.Common.Entities;
using MathSite.Api.Db;
using MathSite.Api.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class EntityApiControllerBase<TEntity> : EntityApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        public EntityApiControllerBase(MathSiteDbContext context, MathServices services)
            : base(context, services)
        {
        }
    }

    public abstract class EntityApiControllerBase<TEntity, TPrimaryKey> : ApiControllerBase
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EntityApiControllerBase(MathSiteDbContext context, MathServices services) : base(context, services)
        {
            Repository = context.Set<TEntity>();
        }

        protected DbSet<TEntity> Repository { get; }
    }
}