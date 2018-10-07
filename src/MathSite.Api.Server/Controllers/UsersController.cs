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
using MathSite.Api.Server.Infrastructure.VersionsAttributes;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Users)]
    [ApiController]
    public class UsersController : CrudPageableApiControllerBase<UserDto, User>
    {
        private readonly IPasswordsManager _passwordsManager;

        public UsersController(
            MathSiteDbContext context,
            IPasswordsManager passwordsManager,
            MathServices services, 
            IMapper mapper
        ) : base(context, services, mapper)
        {
            _passwordsManager = passwordsManager;
        }

        protected override string AreaName { get; } = "Users";

        [HttpGet(MethodNames.Users.GetAll)]
        public async Task<ApiResponse<IEnumerable<UserDto>>> GetAllAsync()
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Users.GetAll, async () =>
            {
                var data = await Repository.Select(user => Mapper.Map<UserDto>(user)).ToArrayAsync();
                return (IEnumerable<UserDto>) data;
            });
        }

        [HttpGet(MethodNames.Users.GetByLogin)]
        public async Task<ApiResponse<UserDto>> GetByLoginAsync(string login)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Users.GetByLogin, async () =>
            {
                var user = await Repository.FirstOrDefaultAsync(u => login == u.Login);
                var userDto = Mapper.Map<UserDto>(user);
                return userDto;
            });
        }

        [HttpGet(MethodNames.Users.GetByLoginAndPassword)]
        public async Task<ApiResponse<UserDto>> GetByLoginAndPasswordAsync(string login, string password)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Users.GetByLoginAndPassword, async () =>
            {
                var user = await Repository.FirstOrDefaultAsync(u => login == u.Login);
                var passwordsAreEqual = _passwordsManager.PasswordsAreEqual(login, password, user.PasswordHash);

                if (!passwordsAreEqual)
                    throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

                var userDto = Mapper.Map<UserDto>(user);
                return userDto;
            });
        }

        [HttpGet(MethodNames.Users.HasRight)]
        public async Task<ApiResponse<bool>> HasRightAsync(Guid userId, string rightAlias)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Users.HasRight, async () =>
            {
                var isGuest = userId == Guid.Empty || await Repository.CountAsync(user => user.Id == userId) == 0;
                
                // если гость, то userId должен быть 100% Empty.
                if (isGuest && userId != Guid.Empty)
                    throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);

                var isAdmin = !isGuest && await Repository.AnyAsync(user => user.Id == userId && user.Group.IsAdmin);

                bool hasRight;

                if (isAdmin)
                {
                    hasRight = await Repository.AnyAsync(user => user.Id == userId && user.UserRights.All(right => right.Allowed));
                }
                else if (isGuest)
                {
                    Services.Groups.ShouldRaiseException = false;
                    var guestsGroup = await Services.Groups.GetByAliasAsync(GroupAliases.Guest);

                    if (guestsGroup.IsNull())
                        throw new MissingMemberException(ExceptionsDescriptions.GuestsGroupNotFound);

                    hasRight = await Services.Groups.HasRightAsync(rightAlias);
                }
                else
                {
                    var count = await Repository.Where(user => user.Id == userId)
                        .CountAsync(user =>
                            user.UserRights.Any(
                                right => right.Right.Alias == rightAlias && right.Allowed
                            )
                        );

                    hasRight = count > 0;
                }
                
                return hasRight;
            });
        }

        [HttpGet(MethodNames.Users.HasCurrentUserRight)]
        public async Task<ApiResponse<bool>> HasCurrentUserRightAsync(string rightAlias)
        {
            return await ExecuteSafelyWithMethodAccessCheck(MethodAccessNames.Users.HasCurrentUserRight, async () =>
            {
                var userId = GetCurrentUserId();

                var allowed = await Services.Users.HasRightAsync(userId, rightAlias);
                return allowed;
            });
        }

        private Guid GetCurrentUserId()
        {
            var user = HttpContext.User;
            Guid id;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                id = Guid.Empty;
            }
            else
            {
                const string claimType = "UserId";

                if (user.HasClaim(claim => claim.Type == claimType && claim.Value.IsNotNullOrWhiteSpace()))
                {
                    var claim = user.Claims.First(c => c.Type == claimType && c.Value.IsNotNullOrWhiteSpace());
                        
                    id = Guid.Parse(claim.Value);
                }
                else
                {
                    id = Guid.Empty;
                }
            }

            return id;
        }

        protected override async Task<User> ViewModelToEntityAsync(UserDto viewModel, ActionType actionType)
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