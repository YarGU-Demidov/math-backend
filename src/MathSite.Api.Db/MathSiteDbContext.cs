using MathSite.Api.Db.EntityConfiguration;
using MathSite.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Db
{
    /// <summary>
    ///     Контекст сайта.
    /// </summary>
    public class MathSiteDbContext : DbContext
    {
        /// <summary>
        ///     Контекст.
        /// </summary>
        /// <param name="options">Настройки контекста.</param>
        public MathSiteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupsRight> GroupsRights { get; set; }
        public DbSet<GroupType> GroupTypes { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostAttachment> PostAttachments { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostGroupsAllowed> PostGroupsAllowed { get; set; }
        public DbSet<PostKeyword> PostKeywords { get; set; }
        public DbSet<PostOwner> PostOwners { get; set; }
        public DbSet<PostRating> PostRatings { get; set; }
        public DbSet<PostSeoSetting> PostSeoSettings { get; set; }
        public DbSet<PostSetting> PostSettings { get; set; }
        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<PostUserAllowed> PostUserAllowed { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UsersRight> UsersRights { get; set; }
        public DbSet<Professor> Professors { get; set; }

        /// <summary>
        ///     Добавление конфигурации сущностей.
        /// </summary>
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyAllConfigurations();

            modelBuilder.HasPostgresExtension("uuid-ossp");
            base.OnModelCreating(modelBuilder);
        }
    }
}