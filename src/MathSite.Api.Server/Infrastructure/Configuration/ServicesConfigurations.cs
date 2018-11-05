using System;
using System.Text;
using AutoMapper;
using MathSite.Api.Common;
using MathSite.Api.Common.ActionResults;
using MathSite.Api.Common.Crypto;
using MathSite.Api.Common.FileFormats;
using MathSite.Api.Common.FileStorage;
using MathSite.Api.Db;
using MathSite.Api.Services.Infrastructure;
using MathSite.Common.ApiServiceRequester.Abstractions;
using MathSite.Common.ApiServiceRequester.UriBuilders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
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
            services.AddStorage<LocalFileSystemStorage>();

            // for uploading really large files.
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddAutoMapper();

            var authSection = configuration.GetSection("Auth");

            services.AddMathApi<AfterDomainServiceUriBuilder>(authSection);

            services.AddOptions();
            services.Configure<ExtendedAuthData>(authSection);

            services.AddScoped<IPasswordsManager, DoubleSha512HashPasswordsManager>();

            return services;
        }

        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration appConfiguration, bool isDevelopment)
        {
            var siteUrl = appConfiguration["Auth:SiteUrl"];
            var issuer = appConfiguration["Auth:Issuer"];
            var audience = appConfiguration["Auth:Audience"];
            var key = appConfiguration["Auth:Key"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var clockSkew = TimeSpan.FromMinutes(5);

            services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(options =>
                {
                    options.Authority = issuer;
                    options.Audience = siteUrl;
                    options.RequireHttpsMetadata = !isDevelopment;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,
                        
                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        RequireSignedTokens = true,

                        ClockSkew = clockSkew
                    };
                    options.Configuration = new OpenIdConnectConfiguration();
                    options.Validate();
                });

            return services;
        }
    }
}