using System;
using MathSite.Api.Common.Entities;
using MathSite.Api.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public class ApiControllerBase<TEntity> : ApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        public ApiControllerBase(MathSiteDbContext context)
            : base(context)
        {
        }
    }

    public class ApiControllerBase<TEntity, TPrimaryKey> : ControllerBase
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ApiControllerBase(MathSiteDbContext context)
        {
            Context = context;
            Repository = context.Set<TEntity>();
        }

        protected DbSet<TEntity> Repository { get; }
        protected MathSiteDbContext Context { get; }
    }
}