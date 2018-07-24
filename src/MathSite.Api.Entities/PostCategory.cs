using System;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    public class PostCategory : Entity
    {
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}