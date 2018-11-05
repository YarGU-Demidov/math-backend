using MathSite.Api.Db;

namespace MathSite.Api.Server.Infrastructure.CommonServiceMethods
{
    public abstract class BaseServiceMethods
    {
        protected BaseServiceMethods(MathSiteDbContext context)
        {
            Context = context;
        }

        protected MathSiteDbContext Context { get; }
    }
}