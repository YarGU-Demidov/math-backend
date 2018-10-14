using System;
using System.Linq;
using System.Threading.Tasks;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.VersionsAttributes;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Auth)]
    [ApiController]
    public class AuthController : ApiControllerBase
    {
        public AuthController(MathSiteDbContext context, MathServices services) : base(context, services)
        {
        }

        protected override string AreaName { get; } = ServiceNames.Auth;

        [HttpGet(MethodNames.Auth.GetCurrentUserId)]
        public Task<ApiResponse<Guid>> GetCurrentUserId()
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

                return Task.FromResult(id);
            });
        }
    }
}