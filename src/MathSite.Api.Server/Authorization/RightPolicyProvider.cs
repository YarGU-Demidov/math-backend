using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MathSite.Api.Server.Authorization
{
    public class RightPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.Split('.').Length != 2) 
                return Task.FromResult<AuthorizationPolicy>(null);

            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new RightRequirement(policyName));

            return Task.FromResult(policy.Build());
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
    }
}