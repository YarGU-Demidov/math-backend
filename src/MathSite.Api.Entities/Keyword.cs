using System.Collections.Generic;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    /// <summary>
    ///     Ключевые слова.
    /// </summary>
    public class Keyword : EntityWithNameAndAlias
    {
        /// <summary>
        ///     Список постов, содержащих это ключевое слово.
        /// </summary>
        public ICollection<PostKeyword> Posts { get; set; } = new List<PostKeyword>();
    }
}