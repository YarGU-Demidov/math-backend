using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Categories
{
    public class CategoryAliasSpecification : Specification<Category>
    {
        private readonly string _categoryAlias;

        public CategoryAliasSpecification(string categoryAlias)
        {
            _categoryAlias = categoryAlias;
        }

        public override Expression<Func<Category, bool>> ToExpression()
        {
            return category => category.Alias == _categoryAlias;
        }
    }
}