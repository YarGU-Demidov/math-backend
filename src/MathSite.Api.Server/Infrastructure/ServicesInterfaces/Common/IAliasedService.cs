using System;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common
{
    public interface IAliasedService<TViewModel> : IAliasedService<TViewModel, Guid>
        where TViewModel : BaseEntity<Guid>
    {
    }

    public interface IAliasedService<TViewModel, TPrimaryKey>
        where TViewModel : BaseEntity<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        [HttpGet(MethodNames.Global.GetByAlias)]
        Task<ApiResponse<TViewModel>> GetByAliasAsync(string alias);
    }
}