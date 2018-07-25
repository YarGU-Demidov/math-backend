using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface ICategoryRepository : IMathSiteEfCoreRepository<Category>
    {
    }

    public class CategoryRepository : MathSiteEfCoreRepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(MathSiteDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}