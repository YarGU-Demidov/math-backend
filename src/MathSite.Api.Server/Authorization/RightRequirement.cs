using Microsoft.AspNetCore.Authorization;

namespace MathSite.Api.Server.Authorization
{
    public class RightRequirement : IAuthorizationRequirement
    {
        public RightRequirement(string right)
        {
            Right = right;
        }

        public string Right { get; private set; }
    }
}