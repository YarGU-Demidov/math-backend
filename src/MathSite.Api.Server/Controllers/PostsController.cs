using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Common.Exceptions;
using MathSite.Api.Common.Extensions;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Db.DataSeeding.StaticData;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using MathSite.Api.Services.Infrastructure.Args;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Posts)]
    [ApiController]
    public class PostsController : EntityApiControllerBase<Post>, IPostsService
    {
        private const string ServiceName = ServiceNames.Posts;

        private readonly CrudServiceMethods<Post, PostDto> _crudServiceMethods;
        private readonly PageableServiceMethods<Post, PostDto> _pageableServiceMethods;
        private readonly CountableServiceMethods<Post> _countableServiceMethods;
        public PostsController(MathSiteDbContext context, MathServices services, IMapper mapper, CountableServiceMethods<Post> countableServiceMethods, CrudServiceMethods<Post, PostDto> crudServiceMethods, PageableServiceMethods<Post, PostDto> pageableServiceMethods) : base(context, services, mapper)
        {
            _countableServiceMethods = countableServiceMethods;
            _crudServiceMethods = crudServiceMethods;
            _pageableServiceMethods = pageableServiceMethods;
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<PostDto>> GetById(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.GetById(id));
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]PostDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpPut(MethodNames.Global.Update)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Update)]
        public Task<ApiResponse<Guid>> UpdateAsync([FromBody]PostDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.UpdateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        [HttpDelete("delete-many")]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse<int>> DeleteManyAsync([FromBody]List<Guid> ids)
        {
            return ExecuteSafely(() =>
            {
                return Repository.Where(x => ids.Contains(x.Id)).DeleteFromQueryAsync();
            });
        }

        [HttpGet(MethodNames.Global.GetCount)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetCount)]
        public Task<ApiResponse<int>> GetCountAsync()
        {
            return ExecuteSafely(() => _countableServiceMethods.GetCountAsync());
        }

        [HttpGet(MethodNames.Global.GetPaged)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetPaged)]
        public Task<ApiResponse<IEnumerable<PostDto>>> GetAllPagedAsync(int page, int perPage)
        {
            return ExecuteSafely(() => _pageableServiceMethods.GetAllPagedAsync(page, perPage));
        }

        [HttpGet("get-all-by-page-nested")]
        [AuthorizeMethod(ServiceName, "get-all-by-page-nested")]
        public Task<ApiResponse<IEnumerable<PostDto>>> GetAllByPageNested(int page, int perPage)
        {
            return ExecuteSafely(async () =>
            {
                page = page >= 1 ? page : 0;
                perPage = perPage > 0 ? perPage : 0;

                var posts = await Repository
                    .Include(p => p.PostType)
                    .Include(p => p.Author)
                    .Include(p => p.Comments)
                    .Include(p => p.GroupsAllowed)
                    .Include(p => p.UsersAllowed)
                    .Include(p => p.PostAttachments)
                    .Include(p => p.PostCategories)
                    .Include(p => p.PostOwners)
                    .Include(p => p.PostRatings)
                    .Include(p => p.PostSeoSetting)
                    .Include(p => p.PostSettings)
                    .Skip(page * perPage)
                    .Take(perPage)
                    .Select(u => Mapper.Map<PostDto>(u))
                    .ToArrayAsync();

                return (IEnumerable<PostDto>)posts;
            });
        }

        [HttpPost(MethodNames.Posts.GetPagesCount)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Posts.GetPagesCount)]
        public Task<ApiResponse<int>> GetPagesCountAsync(PagesCountArgs pagesCountArgs)
        {
            return ExecuteSafely(async () =>
            {
                var posts = GetCommonFilteredPosts(pagesCountArgs);

                if (pagesCountArgs.PostType.IsNotNull() && pagesCountArgs.PostType.Id != Guid.Empty)
                {
                    posts = posts.Where(p => p.PostType.Id == pagesCountArgs.PostType.Id);
                }
            
                if (pagesCountArgs.Category.IsNotNull() && pagesCountArgs.Category.Id != Guid.Empty)
                {
                    posts = posts.Where(p => p.PostCategories.Any(c => c.CategoryId == pagesCountArgs.Category.Id));
                }

                return await posts.CountAsync();
            });
        }

        [HttpPost(MethodNames.Posts.GetPostByUrlAndType)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Posts.GetPostByUrlAndType)]
        public Task<ApiResponse<PostDto>> GetPostByUrlAndTypeAsync(string url, PostTypeDto postType)
        {
            return ExecuteSafely(async () =>
            {
                if (postType.Alias.IsNullOrWhiteSpace())
                {
                    throw new EntityNotFoundException(ExceptionsDescriptions.EntityNotFound);
                }

                var posts = Repository
                    .Include(p => p.PostType)
                    .Include(p => p.PostSettings).AsQueryable();

                var currentUserId = await Services.Auth.GetCurrentUserIdAsync();
                var isGuest = currentUserId == Guid.Empty || (await Services.Users.GetById(currentUserId)).IsNull();

                var hasRightToViewRemovedAndUnpublished
                    = !isGuest && await Services.Users.HasRightAsync(currentUserId,RightAliases.ManageNewsAccess);

                if (!hasRightToViewRemovedAndUnpublished)
                    posts = posts.Where(p => p.Published & !p.Deleted);

                var post = await posts.FirstOrDefaultAsync(p => p.PostType.Alias == postType.Alias);
                return Mapper.Map<PostDto>(post);
            });
        }

        [HttpPost(MethodNames.Posts.GetPosts)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Posts.GetPosts)]
        public Task<ApiResponse<IEnumerable<PostDto>>> GetPostsAsync(PostsGetterArgs postsArgs)
        {
            return ExecuteSafely(async () =>
            {
                var excludedCategories = postsArgs.ExcludedCategories as CategoryDto[] ?? postsArgs.ExcludedCategories?.ToArray();

                var page = postsArgs.Page >= 1 ? postsArgs.Page : 0;//TODO ???
                var perPage = postsArgs.PerPage > 0 ? postsArgs.PerPage : 0;

                var posts = GetCommonFilteredPosts(postsArgs);

                if (postsArgs.PostType.Alias.IsNotNull())
                {
                    posts = posts.Where(p => p.PostType.Alias == postsArgs.PostType.Alias);
                }

                if (postsArgs.CategoryId.HasValue)
                {
                    posts = posts.Where(p => p.PostCategories.Any(c => c.CategoryId == postsArgs.CategoryId));
                }

                if (excludedCategories.IsNotNullOrEmpty())
                {
                    posts = posts.Where(p => excludedCategories.All(c => c.Id != p.Id));
                }

                return (IEnumerable<PostDto>)await posts
                     .Skip(page * perPage).Take(perPage)
                     .Select(p => Mapper.Map<PostDto>(p)).ToArrayAsync();
            });
        }
        protected async Task<Post> ViewModelToEntityAsync(PostDto viewModel, ActionType actionType)
        {
            Post post;
            if (actionType == ActionType.Create)
            {
                post = new Post();
                Mapper.Map(viewModel, post);
            }
            else
            {
                post = await Repository.FirstOrDefaultAsync(c => c.Id == viewModel.Id);

                Mapper.Map(viewModel, post);
            }

            return post;
        }

        private IQueryable<Post> GetCommonFilteredPosts(PostArgs postArgs)
        {
            var posts = Repository
                .Include(p => p.PostSeoSetting)
                .Include(p => p.PostType)
                .Include(p => p.PostSettings).AsQueryable();

            posts =postArgs.ItemAvailableStatus == ItemAvailableStatus.Removed
                ? posts.Where(p => p.Deleted)
                : posts.Where(p => !p.Deleted);

            posts =postArgs.PublishStatus == PublishStatus.Published
                ? posts.Where(p => p.Published)
                : posts.Where(p => !p.Published);

            posts =postArgs.FrontPageStatus == FrontPageStatus.Visible
                ? posts.Where(p => p.PostSettings.PostOnStartPage)
                : posts.Where(p => !p.PostSettings.PostOnStartPage);

            return posts;
        }
    }
}
