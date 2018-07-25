using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;

namespace MathSite.Api.Repositories
{
    public interface IGroupsRepository : IMathSiteEfCoreRepository<Group>
    {
        
    }

    public class GroupsRepository : MathSiteEfCoreRepositoryBase<Group>, IGroupsRepository
    {
        public GroupsRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }
    }
}