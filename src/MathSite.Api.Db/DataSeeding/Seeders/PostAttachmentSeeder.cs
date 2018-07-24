﻿using System.Linq;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Entities;
using Microsoft.Extensions.Logging;

namespace MathSite.Api.Db.DataSeeding.Seeders
{
    public class PostAttachmentSeeder : AbstractSeeder<PostAttachment>
    {
        /// <inheritdoc />
        public PostAttachmentSeeder(ILogger logger, MathSiteDbContext context) : base(logger, context)
        {
        }

        /// <inheritdoc />
        public override string SeedingObjectName { get; } = nameof(PostAttachment);

        /// <inheritdoc />
        protected override void SeedData()
        {
            var firstPostAttachment = CreatePostAttachment(
                GetPostByTittle(PostAliases.FirstPost),
                GetFileByName(FileAliases.FirstFile)
            );

            var secondPostAttachment = CreatePostAttachment(
                GetPostByTittle(PostAliases.SecondPost),
                GetFileByName(FileAliases.SecondFile)
            );

            var postsAttachments = new[]
            {
                firstPostAttachment,
                secondPostAttachment
            };

            Context.PostAttachments.AddRange(postsAttachments);
        }

        private Post GetPostByTittle(string title)
        {
            return Context.Posts.First(post => post.Title == title);
        }

        private File GetFileByName(string name)
        {
            return Context.Files.First(file => file.Name == name);
        }

        private static PostAttachment CreatePostAttachment(Post post, File file)
        {
            return new PostAttachment
            {
                Post = post,
                File = file
            };
        }
    }
}