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
    public enum ActionType
    {
        Update,
        Create
    }

    public abstract class CrudPageableApiControllerBase<TViewModel, TEntity> : CrudPageableApiControllerBase<TViewModel, TEntity, Guid>
        where TViewModel : BaseEntity
        where TEntity : class, IEntity<Guid>
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
        protected MathServices Services { get; }
        protected IMapper Mapper { get; }

        protected abstract string AreaName { get; }

        protected CrudPageableApiControllerBase(MathSiteDbContext context, MathServices services, IMapper mapper) : base(context)
        {
            Services = services;
            Mapper = mapper;
        }

        [HttpPost(MethodNames.Global.GetOne)]
        public virtual async Task<ApiResponse> GetById(TPrimaryKey id)
        {
            try
            {
                await MethodAccessCheck("Read");
                var entity = await GetEntityByIdAsync(id);
                var model = Mapper.Map<TViewModel>(entity);
                return new DataApiResponse<TViewModel>(model);
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }

        [HttpPost(MethodNames.Global.Create)]
        public virtual async Task<ApiResponse> CreateAsync(TViewModel viewModel)
        {
            try
            {
                await MethodAccessCheck("Create");
                var entity = await ViewModelToEntityAsync(viewModel, ActionType.Create);
                var a = await Repository.AddAsync(entity);
                await Context.SaveChangesAsync();

                return new DataApiResponse<TPrimaryKey>(data: a.Entity.Id);
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }

        [HttpPost(MethodNames.Global.Update)]
        public virtual async Task<ApiResponse> UpdateAsync(TViewModel viewModel)
        {
            try
            {
                await MethodAccessCheck("Update");
                var entity = await ViewModelToEntityAsync(viewModel, ActionType.Update);
                await Task.FromResult(Repository.Update(entity));
                await Context.SaveChangesAsync();
                return new DataApiResponse<TPrimaryKey>(data: entity.Id);
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }

        [HttpPost(MethodNames.Global.Delete)]
        public virtual async Task<ApiResponse> DeleteAsync(TPrimaryKey id)
        {
            try
            {
                await MethodAccessCheck("Delete");
                var entity = await GetEntityByIdAsync(id);
                Repository.Remove(entity);
                await Context.SaveChangesAsync();
                return new VoidApiResponse();
            }
            catch (Exception e)
            {
                return new ErrorApiResponse(e.Message);
            }
        }

        private async Task<TEntity> GetEntityByIdAsync(TPrimaryKey id)
        {
            var item = await Repository.FirstOrDefaultAsync(GetIdComparer(id));

            if (item == null)
                throw new EntityNotFoundException(ErrorsDescriptions.EntityNotFound);

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

        protected async Task MethodAccessCheck(string methodName)
        {
            var accessAllowed = await Services.Users.HasCurrentUserRightAsync($"{AreaName}.{methodName}");

            if (accessAllowed)
                return;

            throw new MethodAccessException("You've got no access to this method.");
        }
    }
}