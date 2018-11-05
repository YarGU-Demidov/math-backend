using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common
{
    public interface ICountableService
    {
        [HttpPost(MethodNames.Global.GetCount)]
        Task<ApiResponse<int>> GetCountAsync();
    }
}