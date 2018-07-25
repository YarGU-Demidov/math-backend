using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface ISiteSettingsRepository : IMathSiteEfCoreRepository<SiteSetting>
    {
    }

    public class SiteSettingsRepository : MathSiteEfCoreRepositoryBase<SiteSetting>, ISiteSettingsRepository
    {
        public SiteSettingsRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }
    }
}