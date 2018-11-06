using System.Linq;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using Microsoft.Extensions.DependencyInjection;

namespace MathSite.Api.Server.Infrastructure.Configuration
{
    public static class CommonServiceMethodsExtensions
    {
        public static IServiceCollection AddServiceMethods(this IServiceCollection services)
        {
            var baseEntityType = typeof(BaseEntityServiceMethods<,>);
            var baseType = typeof(BaseServiceMethods);
            var asm = baseType.Assembly;

            var types = asm.GetTypes()
                .Where(type => baseType.IsAssignableFrom(type)) // только наследники базовых
                .Where(type => type != baseType && type != baseEntityType) // не базовые типы
                .ToList();

            types.ForEach(type => services.AddScoped(type));

            return services;
        }
    }
}