using System;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IAuthService
    {
        Task<ApiResponse<Guid>> GetCurrentUserIdAsync();
        Task<ApiResponse<TokenDto>> GetTokenAsync(string login, string password);
    }
}