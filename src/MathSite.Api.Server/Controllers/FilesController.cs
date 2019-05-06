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

        public FilesController(
            IFileFacade fileFacade,
            MathSiteDbContext context,
            MathServices services,
            IMapper mapper
        ) : base(context, services, mapper)
        {
            _fileFacade = fileFacade;
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

        [HttpPost("upload-file")]
        [AuthorizeMethod(ServiceName, "upload-file")]
        public Task<ApiResponse<Guid>> UploadFile(Guid directoryId)
        {
            return ExecuteSafely(async () =>
            {
                 var fileData = Request.Form.Files[0];
                 return  await _fileFacade.SaveFileAsync(fileData.FileName, fileData.OpenReadStream(), directoryId);
            });
        }

        private static string GetFileHashString(Stream data)
        {
            byte[] hash;
            using (var sha = new SHA512Managed())
            {
                hash = sha.ComputeHash(data);
            }

            return hash.Select(b => b.ToString("X2")).Aggregate((f, s) => $"{f}{s}");
        }
    }
}