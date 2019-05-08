using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.SiteSettings)]
    [ApiController]
    public class SettingsController : EntityApiControllerBase<SiteSetting>, ISiteSettingsService
    {
        private const string ServiceName = ServiceNames.SiteSettings;

        public SettingsController(
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper
        ) : base(context, services, mapper)
        {
        }
        [HttpGet(MethodNames.SiteSettings.GetDefaultHomePageTitle)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.GetDefaultHomePageTitle)]
        public Task<ApiResponse<string>> GetDefaultHomePageTitleAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.DefaultHomePageTitle));
        }

        [HttpGet(MethodNames.SiteSettings.GetDefaultNewsPageTitle)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.GetDefaultNewsPageTitle)]
        public Task<ApiResponse<string>> GetDefaultNewsPageTitleAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.DefaultNewsPageTitle));
        }

        [HttpGet(MethodNames.SiteSettings.GetPerPageCount)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.GetPerPageCount)]
        public Task<ApiResponse<int>> GetPerPageCountAsync(int defaultCount = 18)
        {
            return ExecuteSafely(async () =>
            {
                var count = await GetStringSettingAsync(SiteSettingsNames.PerPage) ?? defaultCount.ToString();
                return int.Parse(count);
            });
        }

        [HttpGet(MethodNames.SiteSettings.GetSiteName)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.GetSiteName)]
        public Task<ApiResponse<string>> GetSiteNameAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.SiteName));
        }

        [HttpGet("get-site-settings")]
        [AuthorizeMethod(ServiceName, "get-site-settings")]
        public Task<ApiResponse<SiteSettings>> GetSiteSettings()
        {
            return ExecuteSafely(async () =>
            {
                var siteSettings = new SiteSettings();
                siteSettings.SiteName = await GetStringSettingAsync(SiteSettingsNames.SiteName);
                siteSettings.DefaultTitleForNewsPage = await GetStringSettingAsync(SiteSettingsNames.DefaultNewsPageTitle);
                siteSettings.DefaultTitleForHomePage = await GetStringSettingAsync(SiteSettingsNames.DefaultHomePageTitle);
                siteSettings.PerPageCount = int.Parse(await GetStringSettingAsync(SiteSettingsNames.PerPage));
                siteSettings.TitleDelimiter = await GetStringSettingAsync(SiteSettingsNames.TitleDelimiter);
                return siteSettings;
        });
        }

        [HttpGet(MethodNames.SiteSettings.GetTitleDelimiter)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.GetTitleDelimiter)]
        public Task<ApiResponse<string>> GetTitleDelimiterAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.TitleDelimiter));
        }

        [HttpPost(MethodNames.SiteSettings.SetDefaultHomePageTitle)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.SetDefaultHomePageTitle)]
        public Task<ApiResponse<bool>> SetDefaultHomePageTitle(string title)
        {
            return ExecuteSafely(async() => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                return await SetStringSettingAsync(userId, SiteSettingsNames.DefaultHomePageTitle, title);
                });
        }

        [HttpPost(MethodNames.SiteSettings.GetDefaultNewsPageTitle)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.SetDefaultNewsPageTitle)]
        public Task<ApiResponse<bool>> SetDefaultNewsPageTitle(string title)
        {
            return ExecuteSafely(async () => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                return await SetStringSettingAsync(userId, SiteSettingsNames.DefaultNewsPageTitle, title);
            });
        }

        [HttpPost(MethodNames.SiteSettings.SetPerPageCount)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.SetPerPageCount)]
        public Task<ApiResponse<bool>> SetPerPageCountAsync(string count)
        {
            return ExecuteSafely(async () => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                return await SetStringSettingAsync(userId, SiteSettingsNames.PerPage, count);
            });
        }

        [HttpPost(MethodNames.SiteSettings.SetSiteName)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.SetSiteName)]
        public Task<ApiResponse<bool>> SetSiteName(string siteName)
        {
            return ExecuteSafely(async () => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                var result = await SetStringSettingAsync(userId, SiteSettingsNames.SiteName, siteName);
                return result;
            });
        }

        [HttpPost(MethodNames.SiteSettings.SetTitleDelimiter)]
        [AuthorizeMethod(ServiceName, MethodNames.SiteSettings.SetTitleDelimiter)]
        public Task<ApiResponse<bool>> SetTitleDelimiter(string delimiter)
        {
            return ExecuteSafely(async () => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                return await SetStringSettingAsync(userId, SiteSettingsNames.TitleDelimiter, delimiter);
            });
        }

        [HttpPost("set-site-settings")]
        [AuthorizeMethod(ServiceName, "set-site-settings")]
        public Task<ApiResponse<bool>> SetSiteSettings([FromBody] SiteSettings settings)
        {
            return ExecuteSafely(async () => {
                var userId = await Services.Auth.GetCurrentUserIdAsync();
                var result = await SetStringSettingAsync(userId, SiteSettingsNames.DefaultHomePageTitle, settings.DefaultTitleForHomePage);
                result = result? await SetStringSettingAsync(userId, SiteSettingsNames.DefaultNewsPageTitle, settings.DefaultTitleForNewsPage) : result;
                result = result? await SetStringSettingAsync(userId, SiteSettingsNames.PerPage, settings.PerPageCount.ToString()) : result;
                result = result? await SetStringSettingAsync(userId, SiteSettingsNames.SiteName, settings.SiteName) : result;
                result = result? await SetStringSettingAsync(userId, SiteSettingsNames.TitleDelimiter, settings.TitleDelimiter) : result;
                return result;
            });
        }

        private async Task<string> GetStringSettingAsync(string name)
        {
            var settingValue = await GetValueForKey(setting => setting.Key == name);

            return settingValue;
        }

        private async Task<string> GetValueForKey(Expression<Func<SiteSetting, bool>> expression)
        {
            var setting = await Repository.FirstOrDefaultAsync(expression);

            return setting != null
                ? Encoding.UTF8.GetString(setting.Value)
                : null;
        }
        private async Task<bool> SetStringSettingAsync(Guid userId, string name, string value)
        {
            // muted for better times
            //var userDoesntExist = userId == Guid.Empty || await Context.Users.CountAsync(user => user.Id == userId) == 0;
            //if (userDoesntExist)
            //{
            //    return false;
            //}
            //var hasRight = await Services.Users.HasRightAsync(userId, RightAliases.SetSiteSettingsAccess);
            //if (!hasRight)
            //    return false;

            var setting =
                await Context.SiteSettings.FirstOrDefaultAsync(s => s.Key == name);

            var valueBytes = Encoding.UTF8.GetBytes(value);

            if (setting == null)
            {
                await Context.SiteSettings.AddAsync(new SiteSetting(name, valueBytes));
            }
            else
            {
                setting.Value = valueBytes;
                Context.SiteSettings.Update(setting);
            }
            Context.SaveChanges();
            return true;
        }
    }
}