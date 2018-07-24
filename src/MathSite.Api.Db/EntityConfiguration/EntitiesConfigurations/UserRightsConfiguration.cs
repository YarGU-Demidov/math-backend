﻿using MathSite.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MathSite.Api.Db.EntityConfiguration.EntitiesConfigurations
{
    /// <inheritdoc />
    public class UserRightsConfiguration : AbstractEntityConfiguration<UsersRight>
    {
        protected override string TableName { get; } = "UserRights";
        
        /// <inheritdoc />
        protected override void SetFields(EntityTypeBuilder<UsersRight> modelBuilder)
        {
            base.SetFields(modelBuilder);

            modelBuilder
                .Property(gr => gr.Allowed)
                .IsRequired();
        }

        /// <inheritdoc />
        protected override void SetRelationships(EntityTypeBuilder<UsersRight> modelBuilder)
        {
            base.SetRelationships(modelBuilder);

            modelBuilder
                .HasOne(usersRights => usersRights.User)
                .WithMany(user => user.UserRights)
                .HasForeignKey(usersRights => usersRights.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .HasOne(usersRights => usersRights.Right)
                .WithMany(right => right.UsersRights)
                .HasForeignKey(usersRights => usersRights.RightId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}