using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Db;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public class CountableServiceMethods<TEntity> : CountableServiceMethods<TEntity, Guid>
        where TEntity : Entity<Guid>
    {
        public CountableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }

    public class CountableServiceMethods<TEntity, TPrimaryKey> : BaseEntityServiceMethods<TEntity, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey>
    {
        public CountableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> wherePredicate = default, CancellationToken cancellationToken = default)
        {
            var repo = wherePredicate == null 
                ? Repository 
                : Repository.Where(wherePredicate);

            var count = await repo.CountAsync(cancellationToken);

            return count;
        }
    }
}