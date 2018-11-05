using System;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Internal;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IAuthService
    {
        [HttpGet(MethodNames.Auth.GetCurrentUserId)]
        Task<ApiResponse<Guid>> GetCurrentUserIdAsync();

        [HttpPost(MethodNames.Auth.GetToken)]
        Task<ApiResponse<TokenDto>> GetTokenAsync(string login, string password);
    }
}