using System;
using MathSite.Api.Common.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace MathSite.Api.Server.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeMethodAttribute : AuthorizeAttribute
    {
        public AuthorizeMethodAttribute(string serviceName, string methodName) : base($"{serviceName}.{methodName}")
        {
            if (serviceName.IsNullOrWhiteSpace() || methodName.IsNullOrWhiteSpace())
                throw new AccessViolationException("Service and/or method name was not set");
        }
    }
}