using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public class PageableServiceMethods<TEntity, TViewModel> : PageableServiceMethods<TEntity, TViewModel, Guid>
        where TEntity : Entity<Guid>
        where TViewModel : BaseEntity<Guid>
    {
        public PageableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }

    public class PageableServiceMethods<TEntity, TViewModel, TPrimaryKey> : BaseEntityServiceMethods<TEntity, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey>
        where TViewModel: BaseEntity<TPrimaryKey>
    {
        public PageableServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<TViewModel>> GetAllPagedAsync(
            int page,
            int perPage,
            Expression<Func<TEntity, bool>> wherePredicate = default,
            CancellationToken cancellationToken = default
        )
        {
            page = page >= 1 ? page : 1;
            perPage = perPage > 0 ? perPage : 1;

            var skip = (page - 1) * perPage;

            var repo = wherePredicate == null
                ? Repository
                : Repository.Where(wherePredicate);

            var entities = await repo.PageBy(perPage, skip).ToArrayAsync(cancellationToken);

            var models = entities.Select(entity => Mapper.Map<TEntity, TViewModel>(entity));

            return models;
        }
    }
}