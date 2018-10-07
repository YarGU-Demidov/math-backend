using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Exceptions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class CrudPageableApiControllerBase<TViewModel, TEntity> : CrudPageableApiControllerBase<TViewModel, TEntity, Guid>
        where TViewModel : BaseEntity
        where TEntity : class, IEntity
    {
        protected CrudPageableApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper) : base(context, services, mapper)
        {
        }

        protected override Expression<Func<TEntity, bool>> GetIdComparer(Guid compareToKey)
        {
            return entity => entity.Id == compareToKey;
        }
    }

    public abstract class CrudPageableApiControllerBase<TViewModel, TEntity, TPrimaryKey> : PageableApiControllerBase<TEntity, TPrimaryKey>
        where TViewModel : BaseEntity<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        protected IMapper Mapper { get; }

        protected CrudPageableApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper) : base(context, services)
        {
            Mapper = mapper;
        }

        [HttpPost(MethodNames.Global.GetOne)]
        public virtual async Task<ApiResponse<TViewModel>> GetById(TPrimaryKey id)
        {   
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.GetOne, async () =>
            {
                var entity = await GetEntityByIdAsync(id);
                var model = Mapper.Map<TViewModel>(entity);
                return model;
            });
        }

        [HttpPost(MethodNames.Global.Create)]
        public virtual async Task<ApiResponse<TPrimaryKey>> CreateAsync(TViewModel viewModel)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.Create, async () =>
            {
                var entity = await ViewModelToEntityAsync(viewModel, ActionType.Create);
                var entry = await Repository.AddAsync(entity);
                await Context.SaveChangesAsync();

                return entry.Entity.Id;
            });
        }

        [HttpPost(MethodNames.Global.Update)]
        public virtual async Task<ApiResponse<TPrimaryKey>> UpdateAsync(TViewModel viewModel)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.Update ,async () =>
            {
                var entity = await ViewModelToEntityAsync(viewModel, ActionType.Update);
                await Task.FromResult(Repository.Update(entity));
                await Context.SaveChangesAsync();
                return entity.Id;
            });
        }

        [HttpPost(MethodNames.Global.Delete)]
        public virtual async Task<ApiResponse> DeleteAsync(TPrimaryKey id)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Global.Delete, async () =>
            {
                var entity = await GetEntityByIdAsync(id);
                Repository.Remove(entity);
                await Context.SaveChangesAsync();
                return new VoidApiResponse<string>();
            });
        }

        private async Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        {
            var item = await Repository.FirstOrDefaultAsync(GetIdComparer(id));

            if (item == null)
                throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

            return item;
        }

        protected virtual async Task<TEntity> ViewModelToEntityAsync(TViewModel viewModel, ActionType actionType)
        {
            if (actionType == ActionType.Create)
                return Mapper.Map<TEntity>(viewModel);
            
            var item = await Repository.FirstAsync(GetIdComparer(viewModel.Id));

            return Mapper.Map(viewModel, item);
        }

        /// <summary>
        ///     Получает компаратор, чтоб EntityFramework смог построить корректный запрос.
        /// </summary>
        /// <param name="compareToKey">Ключ, с которым сравниваем</param>
        /// <returns>Выражение для EntityFramework, сравнивающее 2 Id-шника</returns>
        protected abstract Expression<Func<TEntity, bool>> GetIdComparer(TPrimaryKey compareToKey);
    }
}