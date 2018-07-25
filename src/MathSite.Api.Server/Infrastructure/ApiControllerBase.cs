using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Repositories.Core;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure
{
    public class ApiControllerBase<TRepository, TEntity> : ApiControllerBase<TRepository, TEntity, Guid>
        where TEntity : class, IEntity<Guid>
        where TRepository : class, IMathSiteEfCoreRepository<TEntity, Guid>
    {
        public ApiControllerBase(IRepositoryManager repositoryManager)
            : base(repositoryManager)
        {
        }
    }

    public class ApiControllerBase<TRepository, TEntity, TPrimaryKey> : ApiControllerBase
        where TEntity : class, IEntity<TPrimaryKey>
        where TRepository : class, IMathSiteEfCoreRepository<TEntity, TPrimaryKey>
    {
        public ApiControllerBase(IRepositoryManager repositoryManager)
            : base(repositoryManager)
        {
            Repository = repositoryManager.TryGetRepository<TRepository>();
        }

        protected TRepository Repository { get; }

        protected async Task<int> GetCountAsync(
            Expression<Func<TEntity, bool>> requirements
        )
        {
            return await GetCountAsync(requirements, Repository);
        }

        public async Task<IEnumerable<TEntity>> GetItemsForPageAsync(
            Func<TRepository, TRepository> config,
            Expression<Func<TEntity, bool>> requirements,
            int page,
            int perPage
        )
        {
            return await GetItemsForPageAsync(config(Repository), requirements, page, perPage);
        }

        public async Task<IEnumerable<TEntity>> GetItemsForPageAsync(Expression<Func<TEntity, bool>> requirements, int page, int perPage)
        {
            return await GetItemsForPageAsync(Repository, requirements, page, perPage);
        }

        public async Task<IEnumerable<TEntity>> GetItemsForPageAsync(
            Func<TRepository, TRepository> config,
            int page,
            int perPage
        )
        {
            return await GetItemsForPageAsync(config(Repository), new AnySpecification<TEntity>(), page, perPage);
        }

        public async Task<IEnumerable<TEntity>> GetItemsForPageAsync(int page, int perPage)
        {
            return await GetItemsForPageAsync(Repository, new AnySpecification<TEntity>(), page, perPage);
        }
    }

    public abstract class ApiControllerBase : ControllerBase
    {
        public IRepositoryManager RepositoryManager { get; }

        public ApiControllerBase(IRepositoryManager repositoryManager)
        {
            RepositoryManager = repositoryManager;
        }

        protected async Task<int> GetCountAsync<TEntity, TKey>(
            Expression<Func<TEntity, bool>> requirements,
            IRepository<TEntity, TKey> repo
        )
            where TEntity : class, IEntity<TKey>
        {
            
            return await repo.CountAsync(requirements);
        }

        protected int GetPagesCount(int perPage, int totalItems)
        {
            return (int) Math.Ceiling(totalItems / (float) perPage);
        }

        protected async Task<IEnumerable<TEntity>> GetItemsForPageAsync<TEntity, TPrimaryKey>(
            IMathSiteEfCoreRepository<TEntity, TPrimaryKey> repo,
            Expression<Func<TEntity, bool>> requirements,
            int page,
            int perPage
        ) where TEntity : class, IEntity<TPrimaryKey>
        {
            page = page >= 1 ? page : 1;
            perPage = perPage > 0 ? perPage : 1;

            var skip = (page - 1) * perPage;

            return await repo
                .GetAllPagedAsync(requirements, perPage, skip);
        }
    }
}