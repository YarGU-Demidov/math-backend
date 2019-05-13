using MathSite.Api.Core;
using MathSite.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    interface ISiteSettingsService
    {
        Task<ApiResponse<int>> GetPerPageCountAsync(int count);
        Task<ApiResponse<string>> GetTitleDelimiterAsync();
        Task<ApiResponse<string>> GetDefaultHomePageTitleAsync();
        Task<ApiResponse<string>> GetDefaultNewsPageTitleAsync();
        Task<ApiResponse<string>> GetSiteNameAsync();
        Task<ApiResponse<bool>> SetPerPageCountAsync(string count);
        Task<ApiResponse<bool>> SetTitleDelimiter(string delimiter);
        Task<ApiResponse<bool>> SetDefaultHomePageTitle(string title);
        Task<ApiResponse<bool>> SetDefaultNewsPageTitle(string title);
        Task<ApiResponse<bool>> SetSiteName(string siteName);
        Task<ApiResponse<SiteSettings>> GetSiteSettings();
    }
}
