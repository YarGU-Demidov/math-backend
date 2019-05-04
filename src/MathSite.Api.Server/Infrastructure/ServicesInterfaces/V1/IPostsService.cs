using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathSite.Api.Core;
using MathSite.Api.Dto;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.Common;
using MathSite.Api.Services.Infrastructure.Args;
using MathSite.Common.ApiServiceRequester.Abstractions;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IPostsService : ICrudService<PostDto>, IPageableService<PostDto>
    {
        Task<ApiResponse<int>> GetPagesCountAsync(PagesCountArgs pagesCountArgs);

        Task<ApiResponse<PostDto>> GetPostByUrlAndTypeAsync(string url, PostTypeDto postType);

        Task<ApiResponse<IEnumerable<PostDto>>> GetPostsAsync(PostsGetterArgs postsArgs);
    }
}
