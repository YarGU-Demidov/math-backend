using System;
using System.Collections.Generic;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    /// <summary>
    /// </summary>
    public class PostType : EntityWithNameAndAlias
    {
        public Guid DefaultPostsSettingsId { get; set; }
        public PostSetting DefaultPostsSettings { get; set; }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}