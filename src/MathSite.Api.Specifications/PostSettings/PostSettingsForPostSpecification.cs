using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.PostSettings
{
    public class PostSettingsForPostSpecification : Specification<PostSetting>
    {
        private readonly Guid _postId;

        public PostSettingsForPostSpecification(Post post)
        {
            _postId = post.Id;
        }

        public PostSettingsForPostSpecification(Guid postId)
        {
            _postId = postId;
        }

        public override Expression<Func<PostSetting, bool>> ToExpression()
        {
            return setting => setting.Post.Id == _postId;
        }
    }
}