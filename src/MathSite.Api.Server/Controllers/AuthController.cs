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
using MathSite.Api.Entities;
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