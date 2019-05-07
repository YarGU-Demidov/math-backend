using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [DefaultApiRoute(ServiceNames.Files)]
    [ApiController]
    public class FilesController : EntityApiControllerBase<Entities.File>, IFileService
    {
        private const string ServiceName = ServiceNames.Directories;
        private IFileFacade _fileFacade;
        private readonly CrudServiceMethods<Entities.File, FileDto> _crudServiceMethods;

        public FilesController(
            IFileFacade fileFacade,
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper,
            CrudServiceMethods<Entities.File, FileDto> crudServiceMethods
        ) : base(context, services, mapper)
        {
            _fileFacade = fileFacade;
            _crudServiceMethods = crudServiceMethods;
        }

        [HttpPost(MethodNames.Global.Create)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Create)]
        public Task<ApiResponse<Guid>> CreateAsync([FromBody]FileDto viewModel)
        {
            return ExecuteSafely(() => _crudServiceMethods.CreateAsync(viewModel, ViewModelToEntityAsync));
        }


        [HttpDelete(MethodNames.Global.Delete)]
        [AuthorizeMethod(ServiceName, MethodAccessNames.Global.Delete)]
        public Task<ApiResponse> DeleteAsync(Guid id)
        {
            return ExecuteSafely(async() => await _fileFacade.Remove(id));
        }

        public Task<ApiResponse<int>> DeleteManyAsync(List<Guid> ids)
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
                .Where(f => f.DirectoryId == directoryId)
                .Select(f => Mapper.Map<FileDto>(f))
                .ToArrayAsync();
                return (IEnumerable<FileDto>)directories;
            });
        }

        public Task<ApiResponse<FileDto>> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("get-root-files")]
        [AuthorizeMethod(ServiceName, "get-root-files")]
        public Task<ApiResponse<IEnumerable<FileDto>>> GetRootFiles()
        {
            return ExecuteSafely(async () =>
            {
                var directories = await Repository
                .Where(f => f.DirectoryId == null)
                .Select(f => Mapper.Map<FileDto>(f))
                .ToArrayAsync();
                return (IEnumerable<FileDto>)directories;
            });
        }

        public Task<ApiResponse<Guid>> UpdateAsync(FileDto viewModel)
        {
            throw new NotImplementedException();
        }

        [HttpPost("upload-file")]
        [AuthorizeMethod(ServiceName, "upload-file")]
        [Authorize(Roles = "admin")]
        public Task<ApiResponse<Guid>> UploadFile(Guid directoryId)
        {
            return ExecuteSafely(async () =>
            {
                 var fileData = Request.Form.Files[0];
                 return  await _fileFacade.SaveFileAsync(fileData.FileName, fileData.OpenReadStream(), directoryId);
            });
        }

        protected async Task<Entities.File> ViewModelToEntityAsync(FileDto viewModel, ActionType actionType)
        {
            Entities.File file;
            if (actionType == ActionType.Create)
            {
                file = new Entities.File();
                Mapper.Map(viewModel, file);
            }
            else
            {
                file = await Repository.FirstOrDefaultAsync(f => f.Id == viewModel.Id);
                Mapper.Map(viewModel, file);
            }

            return file;
        }
    }
}