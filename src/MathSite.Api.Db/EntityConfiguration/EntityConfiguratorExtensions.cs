using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Db.EntityConfiguration
{
    public static class EntityConfiguratorExtensions
    {
        public static void ApplyAllConfigurations(this ModelBuilder modelBuilder)
        {
            var mi = GetMethodInfo(modelBuilder);
            
            var types = GetAssemblyTypes().GetTypesWithInterfaceImpl();

            var typesWithGenericArg = types.Select(type => (instance: Activator.CreateInstance(type), genericArg: type.GetGenericArgument()))
                .ToList();
            
            typesWithGenericArg.ForEach(
                tuple => mi.MakeGenericMethod(tuple.genericArg)
                    .Invoke(modelBuilder, new[] {tuple.instance})
            );
        }

        private static MethodInfo GetMethodInfo(ModelBuilder modelBuilder)
        {
            var applyConfigurationMethodInfo = modelBuilder
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m => m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
            return applyConfigurationMethodInfo;
        }

        private static Type GetGenericArgument(this Type type)
        {
            var genericType = type;
            while (genericType != typeof(object) && genericType != null)
            {
                var genericArgs = genericType.IsGenericType 
                    ? genericType.GetGenericArguments().ToList() 
                    : Enumerable.Empty<Type>().ToList();

                if (genericArgs.Any())
                    return genericArgs[0];

                genericType = type.BaseType;
            }

            throw new ArgumentException();
        }

        private static IEnumerable<Type> GetTypesWithInterfaceImpl(this IEnumerable<Type> types)
        {
            var interfaceType = typeof(IEntityTypeConfiguration<>);

            return types.Where(
                type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
            ).Where(type => !type.IsAbstract);
        }

        private static IEnumerable<Type> GetAssemblyTypes()
        {
            var asm = typeof(AbstractEntityConfiguration<>).Assembly;
            return asm.GetTypes();
        }
    }
}