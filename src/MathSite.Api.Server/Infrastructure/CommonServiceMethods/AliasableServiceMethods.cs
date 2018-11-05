using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Core;
using MathSite.Api.Db;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public class AliasableServiceMethods<TEntity, TViewModel> : AliasableServiceMethods<TEntity, TViewModel, Guid>
        where TEntity : EntityWithAlias<Guid>
        where TViewModel : BaseEntityWithAlias<Guid>
    {
        public AliasableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }

    public class AliasableServiceMethods<TEntity, TViewModel, TPrimaryKey> : BaseEntityServiceMethods<TEntity, TPrimaryKey>
        where TEntity : EntityWithAlias<TPrimaryKey>
        where TViewModel: BaseEntityWithAlias<TPrimaryKey>
    {
        public AliasableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<TViewModel> GetByAlias(string alias, Expression<Func<TEntity, bool>> wherePredicate = default, CancellationToken cancellationToken = default)
        {
            var repo = wherePredicate == null ? Repository : Repository.Where(wherePredicate);

            var entity = await repo.FirstAsync(e => e.Alias == alias, cancellationToken);
            var model = Mapper.Map<TEntity, TViewModel>(entity);
            return model;
        }
    }
}