﻿using MathSite.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MathSite.Api.Db.EntityConfiguration.EntitiesConfigurations
{
    public class PostConfiguration : AbstractEntityConfiguration<Post>
    {
        protected override string TableName { get; } = nameof(Post);
        
        /// <inheritdoc />
        protected override void SetFields(EntityTypeBuilder<Post> modelBuilder)
        {
            base.SetFields(modelBuilder);

            modelBuilder
                .Property(post => post.Title)
                .IsRequired();

            modelBuilder
                .Property(post => post.Excerpt)
                .IsRequired();

            modelBuilder
                .Property(post => post.Content)
                .IsRequired();

            modelBuilder
                .Property(post => post.PublishDate)
                .IsRequired();

            modelBuilder
                .Property(post => post.Published)
                .IsRequired();

            modelBuilder
                .Property(post => post.Deleted);
        }

        /// <inheritdoc />
        protected override void SetRelationships(EntityTypeBuilder<Post> modelBuilder)
        {
            base.SetRelationships(modelBuilder);

            modelBuilder
                .HasOne(post => post.PostType)
                .WithMany(postType => postType.Posts)
                .HasForeignKey(postType => postType.PostTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.PostCategories)
                .WithOne(postCategory => postCategory.Post)
                .HasForeignKey(postCategory => postCategory.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(post => post.PostSeoSetting)
                .WithOne(postSeoSetting => postSeoSetting.Post)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.PostAttachments)
                .WithOne(postAttachment => postAttachment.Post)
                .HasForeignKey(postAttachment => postAttachment.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(post => post.PostSettings)
                .WithOne(postSetting => postSetting.Post)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.GroupsAllowed)
                .WithOne(groupAllowed => groupAllowed.Post)
                .HasForeignKey(groupAllowed => groupAllowed.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.PostOwners)
                .WithOne(postOwner => postOwner.Post)
                .HasForeignKey(postOwner => postOwner.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.PostRatings)
                .WithOne(postRating => postRating.Post)
                .HasForeignKey(postRating => postRating.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasMany(post => post.UsersAllowed)
                .WithOne(userAllowed => userAllowed.Post)
                .HasForeignKey(userAllowed => userAllowed.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(post => post.Author)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}