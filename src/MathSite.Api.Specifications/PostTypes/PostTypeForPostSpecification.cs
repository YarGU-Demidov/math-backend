using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.PostTypes
{
    public class PostTypeForPostSpecification : Specification<PostType>
    {
        private readonly Guid _postTypeId;

        public PostTypeForPostSpecification(Post post)
        {
            _postTypeId = post.PostTypeId;
        }
        public PostTypeForPostSpecification(Guid postTypeId)
        {
            _postTypeId = postTypeId;
        }

        public override Expression<Func<PostType, bool>> ToExpression()
        {
            return type => type.Id == _postTypeId;
        }
    }
}