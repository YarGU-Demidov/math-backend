using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Core;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Internal;
using MathSite.Api.Server.Infrastructure;
using MathSite.Api.Server.Infrastructure.Attributes;
using MathSite.Api.Server.Infrastructure.Attributes.VersionsAttributes;
using MathSite.Api.Server.Infrastructure.CommonServiceMethods;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Directories)]
    [ApiController]
    public class DirectoriesController : EntityApiControllerBase<Directory>, IDirectoryService
    {
        private const string ServiceName = ServiceNames.Directories;

        private readonly CrudServiceMethods<Directory, DirectoryDto> _crudServiceMethods;

        public DirectoriesController(
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            CrudServiceMethods<Directory, DirectoryDto> crudServiceMethods
        ) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
        }
        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]DirectoryDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }

        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(() => _crudServiceMethods.DeleteAsync(id));
        }

        public Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        [HttpGet(MethodNames.Global.GetOne)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.GetOne)]
        public Task<ApiResponse<DirectoryDto>> GetById(Guid id)
        {
            return ExecuteSafely(async () => await _crudServiceMethods.GetById(id));
        }

        public Task<ApiResponse<Guid>> UpdateAsync(DirectoryDto viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpGet(MethodNames.Users.GetAll)]
        [AuthorizeMethod(ServiceName, MethodNames.Users.GetAll)]
        public Task<ApiResponse<IEnumerable<DirectoryDto>>> GetAllAsync()
        {
            return ExecuteSafely(async () =>
            {
                var directories = await Repository.Select(u => Mapper.Map<DirectoryDto>(u)).ToArrayAsync();
                return (IEnumerable<DirectoryDto>)directories;
            });
        }

        [HttpGet("get-child-directories-by-parent-id")]
        [AuthorizeMethod(ServiceName, "get-child-directories-by-parent-id")]
        public Task<ApiResponse<IEnumerable<DirectoryDto>>> GetChildDirectories(Guid parentId)
        {
            return ExecuteSafely(async () =>
            {
                var directories = await Repository
                .Where(d=>d.RootDirectoryId == parentId)
                .Select(u => Mapper.Map<DirectoryDto>(u))
                .ToArrayAsync();
                return (IEnumerable<DirectoryDto>)directories;
            });
        }

        [HttpGet("get-root-directories")]
        [AuthorizeMethod(ServiceName, "get-root-directories")]
        public Task<ApiResponse<IEnumerable<DirectoryDto>>> GetRootDirectories()
        {
            return ExecuteSafely(async () =>
            {
                var directories = await Repository
                .Where(d => d.RootDirectoryId == null)
                .Select(u => Mapper.Map<DirectoryDto>(u))
                .ToArrayAsync();
                return (IEnumerable<DirectoryDto>)directories;
            });
        }
        protected async Task<Directory> ViewModelToEntityAsync(DirectoryDto viewModel, ActionType actionType)
        {
            Directory directory;
            if (actionType == ActionType.Create)
            {
                directory = new Directory();
                Mapper.Map(viewModel, directory);
            }
            else
            {
                directory = await Repository.FirstOrDefaultAsync(u => u.Id == viewModel.Id);
                Mapper.Map(viewModel, directory);
            }

            return directory;
        }
    }
}