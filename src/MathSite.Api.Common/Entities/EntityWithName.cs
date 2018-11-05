using System;

namespace MathSite.Api.Common.Entities
{
    [Serializable]
    public class EntityWithName : EntityWithName<Guid>
    {
    }

    [Serializable]
    public class EntityWithName<TPrimaryKey> : Entity<TPrimaryKey>
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} | [{nameof(Name)}: {Name}]";
        }
    }
}