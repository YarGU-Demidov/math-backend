using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Crypto;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Entities;
using MathSite.Api.Repositories;
using MathSite.Api.Repositories.Core;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.VersionsAttributes;
using MathSite.Api.Specifications.Users;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute]
    [ApiController]
    public class UsersController : ApiControllerBase<IUsersRepository, User>
    {
        private readonly IUserValidationFacade _validationFacade;
        private readonly IPasswordsManager _passwordsManager;
        public UsersController(
            IRepositoryManager repositoryManager,
            IUserValidationFacade validationFacade,
            IPasswordsManager passwordsManager
        ) : base(repositoryManager)
        {
            _validationFacade = validationFacade;
            _passwordsManager = passwordsManager;
        }

        [HttpGet("get-count")]
        public async Task<ActionResult<int>> GetCountAsync()
        {
            var requirements = new AnySpecification<User>();
            return await GetCountAsync(requirements);
        }

        // TODO: FIXME: Extract to classes or smth else
        public async Task<ActionResult<int>> GetUsersPagesCountAsync(int perPage)
        {
            var requirements = new AnySpecification<User>();
            var usersCount = await GetCountAsync(requirements);

            return GetPagesCount(perPage, usersCount);
        }

        public async Task<ActionResult<IEnumerable<User>>> GetAllUsersAsync()
        {
            return await Repository.WithPerson().GetAllListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int perPage)
        {
            return await GetItemsForPageAsync(repository => repository.WithPerson(), page, perPage);
        }

        public async Task<User> GetUserAsync(string possibleUserId)
        {
            if (possibleUserId.IsNullOrWhiteSpace())
                return null;

            var userIdGuid = Guid.Parse(possibleUserId);

            return await GetUserAsync(userIdGuid);
        }

        public async Task<User> GetUserAsync(Guid possibleUserId)
        {
            if (possibleUserId == default)
                return null;

            return await Repository
                .WithPerson()
                .FirstOrDefaultAsync(possibleUserId);
        }

        public async Task<bool> DoesUserExistsAsync(Guid userId)
        {
            return await Repository.FirstOrDefaultAsync(userId) != null;
        }
        public async Task<bool> DoesUserExistsAsync(string login)
        {
            var requirements = new HasLoginSpecification(login);

            return await RepositoryManager.UsersRepository.FirstOrDefaultAsync(requirements) != null;
        }

        public async Task CreateUserAsync(Guid currentUser, Guid personId, string login, string password, Guid groupId)
        {
            var canCreate = await _validationFacade.UserHasRightAsync(currentUser, RightAliases.AdminAccess);

            if (!canCreate)
                throw new AccessViolationException();

            var passHash = _passwordsManager.CreatePassword(login, password);

            var user = new User(login, passHash, groupId) {PersonId = personId};

            await Repository.InsertAsync(user);
        }
        
        public async Task UpdateUserAsync(Guid currentUser, Guid id, Guid? personId = null, Guid? groupId = null, string newPassword = null)
        {
            var canUpdate = await _validationFacade.UserHasRightAsync(currentUser, RightAliases.AdminAccess);

            if (!canUpdate)
                throw new AccessViolationException();

            var user = await GetUserAsync(id);

            if (newPassword.IsNotNullOrWhiteSpace())
            {
                var passHash = _passwordsManager.CreatePassword(user.Login, newPassword);
                user.PasswordHash = passHash;
            }
            
            if (groupId.HasValue)
                user.GroupId = groupId.Value;

            if (personId.HasValue)
                user.PersonId = personId.Value;

            await Repository.UpdateAsync(user);
        }

        public async Task RemoveUser(Guid currentUser, Guid id)
        {
            var canUpdate = await _validationFacade.UserHasRightAsync(currentUser, RightAliases.AdminAccess);

            if (!canUpdate)
                throw new AccessViolationException();

            await Repository.DeleteAsync(id);
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await Repository.WithPerson().FirstOrDefaultAsync(new HasLoginSpecification(login));
        }
    }
}