using System.Threading.Tasks;
using MathSite.Api.Core;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common
{
    public interface ICountableService
    {
        Task<ApiResponse<int>> GetCountAsync();
    }
}