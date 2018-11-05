using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Auth)]
    [ApiController]
    public class AuthController : ApiControllerBase, IAuthService
    {
        private const string UserIdClaimName = "UserId";

        private readonly ExtendedAuthData _authData;

        public AuthController(
            MathSiteDbContext context, 
            MathServices services,
            IOptions<ExtendedAuthData> authOptions
        ) : base(context, services)
        {
            _authData = authOptions.Value;
        }

        protected const string ServiceName = ServiceNames.Auth;

        [HttpGet(MethodNames.Auth.GetCurrentUserId)]
        public Task<ApiResponse<Guid>> GetCurrentUserIdAsync()
        {
            return ExecuteSafely(() =>
            {
                var user = HttpContext.User;
                Guid id;

                if (user?.Identity == null || !user.Identity.IsAuthenticated)
                {
                    id = Guid.Empty;
                }
                else
                {
                    if (user.HasClaim(claim => claim.Type == UserIdClaimName && claim.Value.IsNotNullOrWhiteSpace()))
                    {
                        var claim = user.Claims.First(c => c.Type == UserIdClaimName && c.Value.IsNotNullOrWhiteSpace());

                        id = Guid.Parse(claim.Value);
                    }
                    else
                    {
                        id = Guid.Empty;
                    }
                }

                return Task.FromResult(id);
            });
        }

        [HttpPost(MethodNames.Auth.GetToken)]
        public Task<ApiResponse<TokenDto>> GetTokenAsync(string login, string password)
        {
            return ExecuteSafely(async () =>
            {
                var identity = await GetIdentityAsync(login, password);

                var now = DateTime.UtcNow;
                var expires = now.Add(TimeSpan.FromMinutes(_authData.Lifetime));
                
                var credentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authData.Key)),
                    SecurityAlgorithms.HmacSha256
                );

                var jst = new JwtSecurityToken(
                    issuer: _authData.Issuer,
                    audience: _authData.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: expires,
                    signingCredentials: credentials
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jst);

                return new TokenDto
                {
                    Login = identity.Name,
                    Token = token,
                    Expires = expires
                };
            });
        }


        public async Task SeedRights()
        {
            var all = new Dictionary<string, string>
            {
                {ServiceNames.Groups, MethodNames.Groups.GetGroupsByType},
                {ServiceNames.Groups, MethodNames.Groups.HasRight},

                {ServiceNames.Auth, MethodNames.Auth.GetCurrentUserId},
                {ServiceNames.Auth, MethodNames.Auth.GetToken},

                {ServiceNames.Directories, MethodNames.Directories.GetDirectoryWithPath},
                {ServiceNames.Directories, MethodNames.Directories.MoveDirectories},

                {ServiceNames.Files, MethodNames.Files.GetFileById},
                {ServiceNames.Files, MethodNames.Files.GetFilesByExtensions},
                {ServiceNames.Files, MethodNames.Files.PutFile},

                {ServiceNames.Persons, MethodNames.Persons.GetAllWithoutProfessors},
                {ServiceNames.Persons, MethodNames.Persons.GetAllWithoutUsers},

                {ServiceNames.PostSeoSettings, MethodNames.PostSeoSettings.GetByPostId},

                {ServiceNames.PostSettings, MethodNames.PostSettings.GetByPostId},

                {ServiceNames.PostTypes, MethodNames.PostTypes.GetByPostId},

                {ServiceNames.Posts, MethodNames.Posts.GetPagesCount},
                
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.GetDefaultHomePageTitle},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.GetDefaultNewsPageTitle},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.GetPerPageCount},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.GetSiteName},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.GetTitleDelimiter},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.SetDefaultHomePageTitle},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.SetDefaultNewsPageTitle},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.SetPerPageCount},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.SetSiteName},
                {ServiceNames.SiteSettings, MethodNames.SiteSettings.SetTitleDelimiter},
            };

            AddCrud(all, ServiceNames.Users);
            AddCrud(all, ServiceNames.Professors);
            AddCrudWithAlias(all, ServiceNames.PostTypes);
            AddCrud(all, ServiceNames.PostSettings);
            AddCrud(all, ServiceNames.PostSeoSettings);
            AddCrud(all, ServiceNames.Posts);
            AddCrud(all, ServiceNames.Persons);
            AddCrudWithAlias(all, ServiceNames.Groups);
            AddCrud(all, ServiceNames.Directories);
            AddCrud(all, ServiceNames.Files);
            AddCrudWithAlias(all, ServiceNames.Categories);
        }
        
        private void AddCountable(Dictionary<string, string> dict, string serviceName)
        {
            dict.Add(serviceName, MethodNames.Global.GetCount);
        }
        
        private void AddPageable(Dictionary<string, string> dict, string serviceName)
        {
            AddCountable(dict, serviceName);
            dict.Add(serviceName, MethodNames.Global.GetPaged);
        }

        private void AddCrud(Dictionary<string, string> dict, string serviceName)
        {
            AddPageable(dict, serviceName);
            dict.Add(serviceName, MethodNames.Global.Create);
            dict.Add(serviceName, MethodNames.Global.Delete);
            dict.Add(serviceName, MethodNames.Global.Update);
            dict.Add(serviceName, MethodNames.Global.GetOne);
        }
        
        private void AddCrudWithAlias(Dictionary<string, string> dict, string serviceName)
        {
            AddCrud(dict, serviceName);
            dict.Add(serviceName, MethodNames.Global.GetByAlias);
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(string login, string password)
        {
            var user = await Services.Users.GetByLoginAndPasswordAsync(login, password);
            var group = await Services.Groups.GetById(user.GroupId);

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, group.Alias),
                new Claim(UserIdClaimName, user.Id.ToString())
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}