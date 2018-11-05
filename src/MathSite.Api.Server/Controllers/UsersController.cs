using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Crypto;
using MathSite.Api.Common.Exceptions;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Users)]
    [ApiController]
    public class UsersController : EntityApiControllerBase<User>, IUserService
    {
        private const string ServiceName = ServiceNames.Users;
        private readonly IPasswordsManager _passwordsManager;
        private readonly CrudServiceMethods<User, UserDto> _crudServiceMethods;
        private readonly PageableServiceMethods<User, UserDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<User> _countableServiceMethods;

        public UsersController(
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            IPasswordsManager passwordsManager,
            CrudServiceMethods<User, UserDto> crudServiceMethods,
            PageableServiceMethods<User, UserDto> pageableServiceMethods,
            CountableServiceMethods<User> countableServiceMethods
        ) : base(context, services, mapper)
        {
            _passwordsManager = passwordsManager;
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
            _countableServiceMethods = countableServiceMethods;
        }

        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<UserDto>> GetById(Guid id)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.GetById(id));
        }

        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync(UserDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync(UserDto viewModel)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [AuthorizeMethod(ServiceName, MethodNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<UserDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [AuthorizeMethod(ServiceName, MethodNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [AuthorizeMethod(ServiceName, MethodNames.Users.GetAll)]
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            return await ExecuteSafely(async () =>
            {
                var data = await Repository.Select(user => Mapper.Map<UserDto>(user)).ToArrayAsync();
                return (IEnumerable<UserDto>) data;
            });
        }

        [AuthorizeMethod(ServiceName, MethodNames.Users.GetByLogin)]
        public async Task<ApiResponse<UserDto>> GetByLoginAsync(string login)
        {
            return await ExecuteSafely(async () =>
            {
                var user = await Repository.FirstOrDefaultAsync(u => login == u.Login);
                var userDto = Mapper.Map<UserDto>(user);
                return userDto;
            });
        }
        
        [AuthorizeMethod(ServiceName, MethodAccessNames.Users.GetByLoginAndPassword)]
        public async Task<ApiResponse<UserDto>> GetByLoginAndPasswordAsync(string login, string password)
        {
            return await ExecuteSafely(async () =>
            {
                var user = await Repository.FirstOrDefaultAsync(u => login == u.Login);
                var passwordsAreEqual = _passwordsManager.PasswordsAreEqual(login, password, user.PasswordHash);

                if (!passwordsAreEqual)
                    throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

                var userDto = Mapper.Map<UserDto>(user);
                return userDto;
            });
        }

        public async Task<ApiResponse<bool>> HasRightAsync(Guid userId, string rightAlias)
        {
            return await ExecuteSafely(async () =>
            {
                var isGuest = userId == Guid.Empty || await Repository.CountAsync(user => user.Id == userId) == 0;

                // если гость, то userId должен быть 100% Empty.
                if (isGuest && userId != Guid.Empty)
                    throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

                var isAdmin = !isGuest && await Repository.AnyAsync(user => user.Id == userId && user.Group.IsAdmin);

                bool hasRight;

                if (isAdmin)
                {
                    hasRight = await Repository.AnyAsync(
                        user => user.Id == userId && user.UserRights.Where(right => right.Right.Alias == rightAlias)
                                    .All(right => right.Allowed)
                    );
                }
                else if (isGuest)
                {
                    Services.Groups.ShouldRaiseException = false;
                    var guestsGroup = await Services.Groups.GetByAliasAsync(GroupAliases.Guest);

                    if (guestsGroup.IsNull())
                        throw new MissingMemberException(ExceptionsDescriptions.GuestsGroupNotFound);

                    hasRight = await Services.Groups.HasRightAsync(guestsGroup.Id, rightAlias);
                }
                else
                {
                    hasRight = await Repository.Where(user => user.Id == userId)
                        .AnyAsync(user =>
                            user.UserRights.Any(
                                right => right.Allowed && right.Right.Alias == rightAlias
                            )
                        );
                }

                return hasRight;
            });
        }

        public async Task<ApiResponse<bool>> HasCurrentUserRightAsync(string rightAlias)
        {
            return await ExecuteSafely(async () =>
            {
                var userId = await Services.Auth.GetCurrentUserIdAsync();

                return await Services.Users.HasRightAsync(userId, rightAlias);
            });
        }

        protected async Task<User> ViewModelToEntityAsync(UserDto viewModel, ActionType actionType)
        {
            User user;
            if (actionType == ActionType.Create)
            {
                user = new User();
                Mapper.Map(viewModel, user);
            }
            else
            {
                user = await Repository.FirstOrDefaultAsync(u => u.Id == viewModel.Id);

                var userLogin = user.Login;
                Mapper.Map(viewModel, user);
                user.Login = userLogin;
            }

            user.PasswordHash = _passwordsManager.CreatePassword(viewModel.Login, viewModel.Password);

            return user;
        }
    }
}