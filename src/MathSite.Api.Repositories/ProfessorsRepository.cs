using MathSite.Api.Db;
using MathSite.Api.Entities;
using MathSite.Api.Repositories.Core;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Repositories
{
    public interface IProfessorsRepository : IMathSiteEfCoreRepository<Professor>
    {
        IProfessorsRepository WithPerson();
    }

    public class ProfessorsRepository : MathSiteEfCoreRepositoryBase<Professor>, IProfessorsRepository
    {
        public ProfessorsRepository(MathSiteDbContext dbContext) : base(dbContext)
        {
        }


        public IProfessorsRepository WithPerson()
        {
            SetCurrentQuery(GetCurrentQuery().Include(professor => professor.Person));
            
            return this;
        }
    }
}