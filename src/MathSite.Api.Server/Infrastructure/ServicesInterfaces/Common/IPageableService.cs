using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common
{
    public interface IPageableService<TViewModel> : IPageableService<TViewModel, Guid>
        where TViewModel : BaseEntity<Guid>
    {
    }

    public interface IPageableService<TViewModel, TPrimaryKey> : ICountableService
        where TViewModel : BaseEntity<TPrimaryKey>
    {
        [HttpPost(MethodNames.Global.GetPaged)]
        Task<ApiResponse<IEnumerable<TViewModel>>> GetAllPagedAsync(int page, int perPage);
    }
}