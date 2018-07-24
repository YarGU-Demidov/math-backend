using System;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    public class PostGroupsAllowed : Entity
    {

        /// <summary>
        /// </summary>
        public bool Allowed { get; set; }

        /// <summary>
        /// </summary>
        public Guid? PostId { get; set; }

        /// <summary>
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// </summary>
        public Group Group { get; set; }
    }
}