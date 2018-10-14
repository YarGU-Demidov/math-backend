using System;

namespace MathSite.Api.Common.Entities
{
    [Serializable]
    public class EntityWithAlias : EntityWithAlias<Guid> { }

    [Serializable]
    public class EntityWithAlias<TPrimaryKey> : Entity<TPrimaryKey>
    {
        public string Alias { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} | [{nameof(Alias)}: {Alias}]";
        }
    }
}