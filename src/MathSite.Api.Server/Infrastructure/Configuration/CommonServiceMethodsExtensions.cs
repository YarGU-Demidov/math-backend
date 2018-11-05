using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using Microsoft.Extensions.DependencyInjection;

namespace MathSite.Api.Server.Infrastructure.Configuration
{
    public static class CommonServiceMethodsExtensions
    {
        public static IServiceCollection AddServiceMethods(this IServiceCollection services)
        {
            return services.AddScoped(typeof(AliasableServiceMethods<,>))
                .AddScoped(typeof(CountableServiceMethods<,>))
                .AddScoped(typeof(CrudServiceMethods<,>))
                .AddScoped(typeof(PageableServiceMethods<,>));
        }
    }
}