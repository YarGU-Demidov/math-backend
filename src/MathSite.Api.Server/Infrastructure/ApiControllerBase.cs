using System;
using System.Threading.Tasks;
using MathSite.Api.Common.Entities;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class ApiControllerBase<TEntity> : ApiControllerBase<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        public ApiControllerBase(MathSiteDbContext context, MathServices services)
            : base(context, services)
        {
        }
    }

    public abstract class ApiControllerBase<TEntity, TPrimaryKey> : ControllerBase
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ApiControllerBase(MathSiteDbContext context, MathServices services)
        {
            Context = context;
            Services = services;
            Repository = context.Set<TEntity>();
        }

        protected DbSet<TEntity> Repository { get; }
        protected MathSiteDbContext Context { get; }
        protected MathServices Services { get; }

        protected abstract string AreaName { get; }

        protected virtual async Task MethodAccessCheck(string methodName)
        {
            var accessAllowed = await Services.Users.HasCurrentUserRightAsync($"{AreaName}.{methodName}");

            if (accessAllowed)
                return;

            throw new MethodAccessException("You've got no access to this method.");
        }

        protected async Task<ApiResponse<T>> ExecuteSafelyWithMethodAccessCheck<T>(string methodName, Func<Task<T>> action)
        {
            return await ExecuteSafely(async () =>
            {
                await MethodAccessCheck(methodName);
                return await action();
            });
        }

        protected async Task<ApiResponse<T>> ExecuteSafely<T>(Func<Task<T>> action)
        {
            try
            {
                var data = await action();
                return new ApiResponse<T>(data);
            }
            catch (Exception e)
            {
                return new ErrorApiResponse<T>(e.Message);
            }
        }
    }
}