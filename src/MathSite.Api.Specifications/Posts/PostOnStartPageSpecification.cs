using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Posts
{
    public class PostOnStartPageSpecification : Specification<Post>
    {
        public override Expression<Func<Post, bool>> ToExpression()
        {
            return post => post.PostSettings.PostOnStartPage;
        }
    }
}