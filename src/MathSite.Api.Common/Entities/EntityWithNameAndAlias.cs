using System;

namespace MathSite.Api.Common.Entities
{

    [Serializable]
    public class EntityWithNameAndAlias : EntityWithNameAndAlias<Guid> { }

    [Serializable]
    public class EntityWithNameAndAlias<TPrimaryKey> : EntityWithAlias<TPrimaryKey>
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} | [{nameof(Name)}: {Name}, {nameof(Alias)}: {Alias}]";
        }
    }
}