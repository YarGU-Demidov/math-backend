using System;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Db;
using MathSite.Api.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class EntityApiControllerBase<TEntity> : EntityApiControllerBase<TEntity, Guid>
        where TEntity : Entity<Guid>
    {
        public EntityApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper)
            : base(context, services, mapper)
        {
        }
    }

    public abstract class EntityApiControllerBase<TEntity, TPrimaryKey> : ApiControllerBase
        where TEntity : Entity<TPrimaryKey>
    {
        protected EntityApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper) : base(
            context, services)
        {
            Mapper = mapper;
            Repository = context.Set<TEntity>();
        }

        protected IMapper Mapper { get; }

        protected DbSet<TEntity> Repository { get; }
    }
}