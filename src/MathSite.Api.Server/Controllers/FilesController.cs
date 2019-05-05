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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Files)]
    [ApiController]
    public class FilesController : EntityApiControllerBase<File>, IFileService
    {
        private const string ServiceName = ServiceNames.Directories;

        private readonly CrudServiceMethods<File, FileDto> _crudServiceMethods;
        public FilesController(
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            CrudServiceMethods<File, FileDto> crudServiceMethods
        ) : base(context, services, mapper)
        {
            _crudServiceMethods = crudServiceMethods;
        }

        public Task<ApiResponse<Guid>> CreateAsync(FileDto viewModel)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<FileDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<Guid>> UpdateAsync(FileDto viewModel)
        {
            throw new NotImplementedException();
        }
        [HttpGet("get-by-directory-id")]
        [AuthorizeMethod(ServiceName, "get-by-directory-id")]
        public Task<ApiResponse<IEnumerable<FileDto>>> GetByDirectoryId(Guid directoryId)
        {
            return ExecuteSafely(async () =>
            {
                var directories = await Repository
                .Where(f=> f.DirectoryId == directoryId)
                .Select(f => Mapper.Map<FileDto>(f))
                .ToArrayAsync();
                return (IEnumerable<FileDto>)directories;
            });
        }
    }
}