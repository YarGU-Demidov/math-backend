using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Db;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public abstract class BaseEntityServiceMethods<TEntity, TPrimaryKey> : BaseServiceMethods
        where TEntity : Entity<TPrimaryKey>
    {
        protected BaseEntityServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context)
        {
            Mapper = mapper;
            Repository = context.Set<TEntity>();
        }

        protected IMapper Mapper { get; }

        protected DbSet<TEntity> Repository { get; }
    }
}