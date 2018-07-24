using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Entities;
using MathSite.Api.Common.Specifications;

namespace MathSite.Api.Specifications
{
    public class SameAliasSpecification<TEntity> : SameAliasSpecification<TEntity, Guid> where TEntity : IEntityWithAlias<Guid>
    {
        public SameAliasSpecification(string alias) : base(alias)
        {
        }
    }

    public class SameAliasSpecification<TEntity, TKey> : Specification<TEntity>
        where TEntity : IEntityWithAlias<TKey>
    {
        private readonly string _alias;

        public SameAliasSpecification(string alias)
        {
            _alias = alias;
        }

        public override Expression<Func<TEntity, bool>> ToExpression()
        {
            return entity => entity.Alias == _alias;
        }
    }
}
