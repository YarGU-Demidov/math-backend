using System;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase(MathSiteDbContext context, MathServices services)
        {
            Context = context;
            Services = services;
        }

        protected MathSiteDbContext Context { get; }
        protected MathServices Services { get; }

        protected async Task<ApiResponse<T>> ExecuteSafely<T>(Func<Task<T>> action)
        {
            try
            {
                var data = await action();
                return new ApiResponse<T>(data);
            }
            catch (Exception e)
            {
                return new ErrorApiResponse<T>(e.Message, e.ToString());
            }
        }

        protected async Task<ApiResponse> ExecuteSafely(Func<Task> voidAction)
        {
            try
            {
                await voidAction();
                return new VoidApiResponse<string>();
            }
            catch (Exception e)
            {
                return new ErrorApiResponse<string>(e.Message, e.ToString());
            }
        }
    }
}