using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MathSite.Api.Server.Authorization
{
    public class RightsAuthorizationHandler : AuthorizationHandler<RightRequirement>
    {
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, RightRequirement requirement)
        {
            //TODO: VALIDATE USER!!!
            context.Succeed(requirement);
        }
    }
}