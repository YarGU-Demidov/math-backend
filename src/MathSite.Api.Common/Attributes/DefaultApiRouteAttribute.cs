using System;

namespace MathSite.Api.Common.Attributes
{
    /// <summary>
    ///     Default Web API route provider with keeping API version in mind.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DefaultApiRouteAttribute : VersionedRouteAttribute
    {
        /// <summary>
        ///     Sets default route for api.
        /// </summary>
        public DefaultApiRouteAttribute()
            : base("[controller]")
        {
        }

        /// <summary>
        ///     Allows to set custom path for route.
        /// </summary>
        /// <param name="additionalPath">
        ///     Your custom url path after version and controller name (if you wont change rewrite
        ///     option).
        /// </param>
        /// <param name="rewrite">Rewrite controller name.</param>
        public DefaultApiRouteAttribute(string additionalPath, bool rewrite = true)
            : base(rewrite ? additionalPath : $"[controller]/{additionalPath}")
        {
        }
    }
}