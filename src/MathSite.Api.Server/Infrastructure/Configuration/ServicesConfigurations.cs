using AutoMapper;
using MathSite.Api.Common;
using MathSite.Api.Common.ActionResults;
using MathSite.Api.Common.FileFormats;
using MathSite.Api.Common.FileStorage;
using MathSite.Api.Db;
using MathSite.Api.Repositories.Core;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MathSite.Api.Server.Infrastructure.Configuration
{
    public static class ServicesConfigurations
    {
        public static IServiceCollection ConfigureMvc(this IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => { options.SerializerSettings.Formatting = Formatting.Indented; });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressInferBindingSourcesForParameters = true;
            });


            return services;
        }

        public static IServiceCollection ConfigureRouting(this IServiceCollection services)
        {
            services.AddRouting(options => { options.LowercaseUrls = true; });

            return services;
        }

        public static IServiceCollection ConfigureDb(this IServiceCollection services, string connectionString, bool isDevelopment)
        {
            services.AddDbContextPool<MathSiteDbContext>(options =>
            {
                var migrationAssemblyName = typeof(Startup).Assembly.GetName().Name;

                options.UseNpgsql(
                    connectionString,
                    builder => builder.MigrationsAssembly(migrationAssemblyName)
                );

                if (isDevelopment)
                    options.EnableSensitiveDataLogging().ConfigureWarnings(builder => builder.Log());
            });

            return services;
        }
        
        public static IServiceCollection ConfigureDi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging();
            services.AddLazyProvider();
            services.AddActionResultExecutors();

            services.Configure<Settings>(configuration);
            services.AddSingleton<FileFormatBuilder>();
            services.AddRepositories();
            services.AddStorage<LocalFileSystemStorage>();

            // for uploading really large files.
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddAutoMapper();

            return services;
        }
    }
}