using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace MathSite.Api.Server.Infrastructure.Configuration
{
    public static class SiteConfiguration
    {
        public static IApplicationBuilder UseDevelopmentConfig(this IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });

            return app;
        }

        public static IApplicationBuilder UseForwarding(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            return app;
        }

        public static IApplicationBuilder UseMathRouting(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "/",
                    defaults: new {controller = "Home", action = "Index"}
                );
            });

            return app;
        }
    }
}