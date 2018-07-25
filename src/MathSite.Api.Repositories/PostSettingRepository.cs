using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface IPostSettingRepository : IMathSiteEfCoreRepository<PostSetting>
    {
    }

    public class PostSettingRepository : MathSiteEfCoreRepositoryBase<PostSetting>, IPostSettingRepository
    {
        public PostSettingRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }
    }
}