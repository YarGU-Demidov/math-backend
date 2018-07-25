using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface IGroupTypeRepository : IMathSiteEfCoreRepository<GroupType>
    {
    }

    public class GroupTypeRepository : MathSiteEfCoreRepositoryBase<GroupType>, IGroupTypeRepository
    {
        public GroupTypeRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }
    }
}