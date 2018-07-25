using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Repositories
{
    public interface IPostSeoSettingsRepository : IMathSiteEfCoreRepository<PostSeoSetting>
    {
        IPostSeoSettingsRepository WithPost();
    }

    public class PostSeoSettingsRepository : MathSiteEfCoreRepositoryBase<PostSeoSetting>, IPostSeoSettingsRepository
    {
        public PostSeoSettingsRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }

        public IPostSeoSettingsRepository WithPost()
        {
            SetCurrentQuery(GetCurrentQuery().Include(setting => setting.Post));
            return this;
        }
    }
}