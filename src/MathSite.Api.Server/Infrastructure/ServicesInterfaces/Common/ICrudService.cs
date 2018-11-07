using System;
using System.Threading.Tasks;
using MathSite.Api.Core;

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
        Task<ApiResponse<TViewModel>> GetById(TPrimaryKey id);
        Task<ApiResponse<TPrimaryKey>> CreateAsync(TViewModel viewModel);
        Task<ApiResponse<TPrimaryKey>> UpdateAsync(TViewModel viewModel);
        Task<ApiResponse> DeleteAsync(TPrimaryKey id);
    }
}