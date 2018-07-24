using System;
using System.Linq;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Posts
{
    public class PostHasCategorySpecification : Specification<Post>
    {
        private readonly Guid _categoryId;

        public PostHasCategorySpecification(Guid categoryId)
        {
            _categoryId = categoryId;
        }

        public override Expression<Func<Post, bool>> ToExpression()
        {
            return post => post.PostCategories.Any(
                category => category.CategoryId == _categoryId
            );
        }
    }
}