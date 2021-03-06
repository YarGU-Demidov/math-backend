﻿using System;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes
{
    /// <summary>
    ///     First API version for app.
    /// </summary>
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class V1Attribute : ApiVersionAttribute
    {
        public V1Attribute() : base(new ApiVersion(1, 0))
        {
            Deprecated = false;
        }
    }
}