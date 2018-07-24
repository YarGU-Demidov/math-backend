using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Posts
{
    public class PostDeletedSpecification : Specification<Post>
    {
        public override Expression<Func<Post, bool>> ToExpression()
        {
            return post => post.Deleted;
        }
    }
}