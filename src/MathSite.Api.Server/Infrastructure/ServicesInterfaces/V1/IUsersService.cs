﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IUsersService : ICrudService<UserDto>, IPageableService<UserDto>, ICountableService
    {
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<UserDto>>> GetByLoginAsync(string login);
        Task<ApiResponse<UserDto>> GetByLoginAndPasswordAsync(string login, string password);
        Task<ApiResponse<bool>> HasRightAsync(Guid userId, string rightAlias);
        Task<ApiResponse<bool>> HasCurrentUserRightAsync(string rightAlias);
    }
}