using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IUserService : ICrudService<UserDto>, IPageableService<UserDto>
    {
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync(bool withPerson = false);
        Task<ApiResponse<UserDto>> GetByLoginAsync(string login);
        Task<ApiResponse<UserDto>> GetByLoginAndPasswordAsync(string login, string password);
        Task<ApiResponse<IEnumerable<UserDto>>> GetAllPagedWithPersonAsync(int page, int perPage);
        Task<ApiResponse<bool>> HasRightAsync(Guid userId, string rightAlias);
        Task<ApiResponse<bool>> HasCurrentUserRightAsync(string rightAlias);
        Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids);
    }
}