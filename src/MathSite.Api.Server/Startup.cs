using MathSite.Api.Server.Infrastructure.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MathSite.Api.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        /// <summary>
        ///     Конфигурация сервисов для разработки.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services, true);
        }

        /// <summary>
        ///     Конфигурация сервисов для боевого сайта.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureProductionServices(IServiceCollection services)
        {
            ConfigureServices(services, false);
        }

        /// <summary>
        ///     Конфигурация сервисов для тестирования.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureStagingServices(IServiceCollection services)
        {
            ConfigureServices(services, false);
        }

        /// <summary>
        ///     Конфигурирование DI и настройка сервисов.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="isDevelopment"></param>
        private void ConfigureServices(IServiceCollection services, bool isDevelopment)
        {
            services.ConfigureMvc();
            services.AddApiVersioning();
            services.ConfigureRouting();
            services.ConfigureDb(Configuration.GetConnectionString("Math"), isDevelopment);
            services.ConfigureDi(Configuration);
            services.ConfigureAuth(Configuration, isDevelopment);
            services.AddServiceMethods();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment()) 
                app.UseDevelopmentConfig();
            app.UseForwarding();
            app.UseAuthentication();
            app.UseMathRouting();
        }
    }
}