using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface IRightsRepository : IMathSiteEfCoreRepository<Right>
    {
        
    }

    public class RightsRepository : MathSiteEfCoreRepositoryBase<Right>, IRightsRepository
    {
        public RightsRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }
    }
}