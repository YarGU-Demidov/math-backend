using System;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class VersionedRouteAttribute : RouteAttribute
    {
        public VersionedRouteAttribute(string template) 
            : base($"v{{version:apiVersion}}/{template}")
        {
        }
    }
}