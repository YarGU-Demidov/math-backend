using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IUserService : ICrudService<UserDto>, IPageableService<UserDto>
    {
        [HttpGet(MethodNames.Users.GetAll)]
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
        
        [HttpGet(MethodNames.Users.GetByLogin)]
        Task<ApiResponse<UserDto>> GetByLoginAsync(string login);
        
        [HttpPost(MethodNames.Users.GetByLoginAndPassword)]
        Task<ApiResponse<UserDto>> GetByLoginAndPasswordAsync(string login, string password);

        [HttpGet(MethodNames.Users.HasRight)]
        Task<ApiResponse<bool>> HasRightAsync(Guid userId, string rightAlias);

        [HttpGet(MethodNames.Users.HasCurrentUserRight)]
        Task<ApiResponse<bool>> HasCurrentUserRightAsync(string rightAlias);
    }
}