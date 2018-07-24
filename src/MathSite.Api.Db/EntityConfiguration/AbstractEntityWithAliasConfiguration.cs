using System;
using MathSite.Api.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MathSite.Api.Db.EntityConfiguration
{
    public abstract class AbstractEntityWithAliasConfiguration<T> : AbstractEntityWithAliasConfiguration<T, Guid> where T : class, IEntityWithAlias<Guid> { }

    public abstract class AbstractEntityWithAliasConfiguration<T, TEntityKey> : AbstractEntityConfiguration<T, TEntityKey>
        where T : class, IEntityWithAlias<TEntityKey>
    {
        protected override void SetFields(EntityTypeBuilder<T> modelBuilder)
        {
            base.SetFields(modelBuilder);
            modelBuilder.Property(model => model.Alias)
                .IsRequired();
        }

        protected override void SetIndexes(EntityTypeBuilder<T> modelBuilder)
        {
            modelBuilder.HasIndex(model => model.Alias)
                .IsUnique();
        }
    }
}