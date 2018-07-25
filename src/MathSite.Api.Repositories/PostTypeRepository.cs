using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Repositories
{
    public interface IPostTypeRepository : IMathSiteEfCoreRepository<PostType>
    {
        IPostTypeRepository WithDefaultPostSettings();
    }

    public class PostTypeRepository : MathSiteEfCoreRepositoryBase<PostType>, IPostTypeRepository
    {
        public PostTypeRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }

        public IPostTypeRepository WithDefaultPostSettings()
        {
            SetCurrentQuery(GetCurrentQuery().Include(type => type.DefaultPostsSettings));
            return this;
        }
    }
}