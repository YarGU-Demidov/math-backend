using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Crypto;
using MathSite.Api.Db;
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
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await Repository.Select(user => Mapper.Map<UserDto>(user)).ToArrayAsync();
        }

        [HttpGet(MethodNames.Users.GetByLogin)]
        public async Task<UserDto> GetByLogin(string login)
        {
            var user = await Repository.FirstOrDefaultAsync(u => login == u.Login);
            return Mapper.Map<UserDto>(user);
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