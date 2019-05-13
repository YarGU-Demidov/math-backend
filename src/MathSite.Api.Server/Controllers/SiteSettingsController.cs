using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Extensions;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.SiteSettings)]
    [ApiController]
    public class SiteSettingsController: EntityApiControllerBase<SiteSetting>, ISiteSettingsService
    {
        public SiteSettingsController(MathSiteDbContext context, MathServices services, IMapper mapper) : base(context, services, mapper)
        {
        }

        protected const string ServiceName = ServiceNames.SiteSettings;

        [HttpGet(MethodNames.SiteSettings.GetPerPageCount)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.GetPerPageCount)]
        public Task<ApiResponse<int>> GetPerPageCountAsync(int count = 18)
        {
            return ExecuteSafely(async () => int.Parse(await GetStringSettingAsync(SiteSettingsNames.PerPage) ?? count.ToString()));
        }

        [HttpGet(MethodNames.SiteSettings.GetTitleDelimiter)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.GetTitleDelimiter)]
        public Task<ApiResponse<string>> GetTitleDelimiterAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.TitleDelimiter));
        }

        [HttpGet(MethodNames.SiteSettings.GetDefaultHomePageTitle)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.GetDefaultHomePageTitle)]
        public Task<ApiResponse<string>> GetDefaultHomePageTitleAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.DefaultHomePageTitle));
        }

        [HttpGet(MethodNames.SiteSettings.GetDefaultNewsPageTitle)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.GetDefaultNewsPageTitle)]
        public Task<ApiResponse<string>> GetDefaultNewsPageTitleAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.DefaultNewsPageTitle));
        }

        [HttpGet(MethodNames.SiteSettings.GetSiteName)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.GetSiteName)]
        public Task<ApiResponse<string>> GetSiteNameAsync()
        {
            return ExecuteSafely(() => GetStringSettingAsync(SiteSettingsNames.SiteName));
        }

        [HttpGet(MethodNames.SiteSettings.SetPerPageCount)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.SetPerPageCount)]
        public Task<ApiResponse<bool>> SetPerPageCountAsync(string count)
        {
            return ExecuteSafely(() => SetStringSettingAsync(SiteSettingsNames.PerPage, count));
        }

        [HttpGet(MethodNames.SiteSettings.SetTitleDelimiter)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.SetTitleDelimiter)]
        public Task<ApiResponse<bool>> SetTitleDelimiter(string delimiter)
        {
            return ExecuteSafely(() => SetStringSettingAsync(SiteSettingsNames.TitleDelimiter, delimiter));
        }

        [HttpGet(MethodNames.SiteSettings.SetDefaultHomePageTitle)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.SetDefaultHomePageTitle)]
        public Task<ApiResponse<bool>> SetDefaultHomePageTitle(string title)
        {
            return ExecuteSafely(() => SetStringSettingAsync(SiteSettingsNames.DefaultHomePageTitle, title));
        }

        [HttpGet(MethodNames.SiteSettings.SetDefaultNewsPageTitle)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.SetDefaultNewsPageTitle)]
        public Task<ApiResponse<bool>> SetDefaultNewsPageTitle(string title)
        {
            return ExecuteSafely(() => SetStringSettingAsync(SiteSettingsNames.DefaultNewsPageTitle, title));
        }

        [HttpGet(MethodNames.SiteSettings.SetSiteName)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.SiteSettings.SetSiteName)]
        public Task<ApiResponse<bool>> SetSiteName(string siteName)
        {
            return ExecuteSafely(() => SetStringSettingAsync(SiteSettingsNames.SiteName, siteName));
        }
        private async Task<string> GetStringSettingAsync(string name)
        {
            var settingValue = await GetValueForKey(name);

            return settingValue;
        }
        [HttpPost("set-site-settings")]
        [AuthorizeMethod(ServiceName, "set-site-settings")]
        public Task<ApiResponse<bool>> SetSiteSettings([FromBody] SiteSettings settings)
        {
            return ExecuteSafely(async () => {
                var result = await SetStringSettingAsync(SiteSettingsNames.DefaultHomePageTitle, settings.DefaultTitleForHomePage);
                result = result ? await SetStringSettingAsync( SiteSettingsNames.DefaultNewsPageTitle, settings.DefaultTitleForNewsPage) : result;
                result = result ? await SetStringSettingAsync(SiteSettingsNames.PerPage, settings.PerPageCount.ToString()) : result;
                result = result ? await SetStringSettingAsync(SiteSettingsNames.SiteName, settings.SiteName) : result;
                result = result ? await SetStringSettingAsync(SiteSettingsNames.TitleDelimiter, settings.TitleDelimiter) : result;
                return result;
            });
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
        private async Task<bool> SetStringSettingAsync(string name, string value)
        {
            // commented till requester fiexed.
            //var currentUserId = await Services.Auth.GetCurrentUserIdAsync();
            //var isGuest = currentUserId == Guid.Empty || (await Services.Users.GetById(currentUserId)).IsNull();

            //if (isGuest)
            //    return false;

            //var hasRight = await Services.Users.HasRightAsync(currentUserId, RightAliases.SetSiteSettingsAccess);
            //if (!hasRight)
            //    return false;

            var setting =
                await Repository.FirstOrDefaultAsync(s=>s.Key == name);

            var valueBytes = Encoding.UTF8.GetBytes(value);

            if (setting == null)
            {
                await Repository.AddAsync(new SiteSetting(name, valueBytes));
            }
            else
            {
                setting.Value = valueBytes;
                Repository.Update(setting);
                await Context.SaveChangesAsync();
            }

            return true;
        }

        private async Task<string> GetValueForKey(string name)
        {
            var setting = await Repository.FirstOrDefaultAsync(s => s.Key == name);

            return setting != null
                ? Encoding.UTF8.GetString(setting.Value)
                : null;
        }

    }
}
