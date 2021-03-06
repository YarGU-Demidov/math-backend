﻿using Microsoft.Extensions.DependencyInjection;

namespace MathSite.Api.Common.ActionResults
{
    public static class ActionsRegisterExtension
    {
        public static IServiceCollection AddActionResultExecutors(this IServiceCollection services)
        {
            return services.AddTransient<FileContentInlineResultExecutor>()
                .AddTransient<FileStreamInlineResultExecutor>();
        }
    }
}