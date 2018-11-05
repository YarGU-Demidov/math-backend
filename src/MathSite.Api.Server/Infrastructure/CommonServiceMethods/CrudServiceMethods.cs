using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Exceptions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public class CrudServiceMethods<TEntity, TViewModel> : CrudServiceMethods<TEntity, TViewModel, Guid>
        where TEntity : Entity<Guid>
        where TViewModel : BaseEntity<Guid>
    {
        private static readonly Func<Guid, Expression<Func<TEntity, bool>>> IdEqualsComparerGetter =
            compareToKey => entity => entity.Id == compareToKey;

        public CrudServiceMethods(MathSiteDbContext context, IMapper mapper) : base(context, mapper, IdEqualsComparerGetter)
        {
        }
    }

    public class CrudServiceMethods<TEntity, TViewModel, TPrimaryKey> : BaseEntityServiceMethods<TEntity, TPrimaryKey>
        where TEntity : Entity<TPrimaryKey>
        where TViewModel : BaseEntity<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        public CrudServiceMethods(
            MathSiteDbContext context,
            IMapper mapper,
            Func<TPrimaryKey, Expression<Func<TEntity, bool>>> idEqualityComparerGetter
        ) : base(context, mapper)
        {
            GetIdEqualityComparer = idEqualityComparerGetter;
        }

        private Func<TPrimaryKey, Expression<Func<TEntity, bool>>> GetIdEqualityComparer { get; }

        public async Task<TViewModel> GetById(TPrimaryKey id)
        {
            var entity = await GetEntityByIdAsync(id);
            var model = Mapper.Map<TViewModel>(entity);
            return model;
        }

        public async Task<TPrimaryKey> CreateAsync(
            TViewModel viewModel,
            Func<TViewModel, ActionType, Task<TEntity>> viewModelToEntityAsync = default
        )
        {
            var asyncConverter = GetAsyncConverter(viewModelToEntityAsync);

            var entity = await asyncConverter(viewModel, ActionType.Create);
            var entry = await Repository.AddAsync(entity);
            await Context.SaveChangesAsync();

            return entry.Entity.Id;
        }

        public async Task<TPrimaryKey> UpdateAsync(
            TViewModel viewModel,
            Func<TViewModel, ActionType, Task<TEntity>> viewModelToEntityAsync = default
        )
        {
            var asyncConverter = GetAsyncConverter(viewModelToEntityAsync);

            var entity = await asyncConverter(viewModel, ActionType.Update);
            var entry = Repository.Update(entity);
            await Context.SaveChangesAsync();
            return entry.Entity.Id;
        }

        public async Task DeleteAsync(TPrimaryKey id)
        {
            // TODO: Надо бы потом переписать, тут могут быть значительные просадки в производительности
            // TODO: Чтения из базы можно избежать, думаю.
            var entity = await GetEntityByIdAsync(id);
            Repository.Remove(entity);
            await Context.SaveChangesAsync();
        }

        private async Task<TEntity> GetEntityByIdAsync(TPrimaryKey id, bool track = false)
        {
            var repo = track ? Repository : Repository.AsNoTracking();

            var item = await repo.FirstOrDefaultAsync(GetIdEqualityComparer(id));

            if (item == null)
                throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

            return item;
        }

        private Func<TViewModel, ActionType, Task<TEntity>> GetAsyncConverter(
            Func<TViewModel, ActionType, Task<TEntity>> func
        )
        {
            return func ?? ViewModelToEntityAsync;
        }

        private async Task<TEntity> ViewModelToEntityAsync(TViewModel viewModel, ActionType actionType)
        {
            if (actionType == ActionType.Create)
                return Mapper.Map<TEntity>(viewModel);

            var item = await Repository.FirstAsync(GetIdEqualityComparer(viewModel.Id));

            return Mapper.Map(viewModel, item);
        }
    }
}