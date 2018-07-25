using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface IPostCategoryRepository : IMathSiteEfCoreRepository<PostCategory>
    {
    }

    public class PostCategoryRepository : MathSiteEfCoreRepositoryBase<PostCategory>, IPostCategoryRepository
    {
        public PostCategoryRepository(MathSiteDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
