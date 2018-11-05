using System;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common
{
    public interface ICrudService<TViewModel> : ICrudService<TViewModel, Guid>
        where TViewModel : BaseEntity<Guid>
    {
    }

    public interface ICrudService<TViewModel, TPrimaryKey>
        where TViewModel : BaseEntity<TPrimaryKey>
        where TPrimaryKey : IComparable<TPrimaryKey>
    {
        [HttpGet(MethodNames.Global.GetOne)]
        Task<ApiResponse<TViewModel>> GetById(TPrimaryKey id);

        [HttpPost(MethodNames.Global.Create)]
        Task<ApiResponse<TPrimaryKey>> CreateAsync(TViewModel viewModel);

        [HttpPost(MethodNames.Global.Update)]
        Task<ApiResponse<TPrimaryKey>> UpdateAsync(TViewModel viewModel);

        [HttpPost(MethodNames.Global.Delete)]
        Task<ApiResponse> DeleteAsync(TPrimaryKey id);
    }
}