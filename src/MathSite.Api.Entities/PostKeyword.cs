using System;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    public class PostKeyword : Entity
    {
        public Guid KeywordId { get; set; }
        public Keyword Keyword { get; set; }
        public Guid PostSeoSettingsId { get; set; }
        public PostSeoSetting PostSeoSettings { get; set; }
    }
}