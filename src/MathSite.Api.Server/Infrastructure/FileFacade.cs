using AutoMapper;
using MathSite.Api.Common.Exceptions;
using MathSite.Api.Common.FileStorage;
using MathSite.Api.Db;
using MathSite.Api.Dto;
using MathSite.Api.Entities;
using MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1;
using MathSite.Api.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading.Tasks;
using File = MathSite.Api.Entities.File;

namespace MathSite.Api.Server.Infrastructure
{
    public class FileFacade : IFileFacade
    {
        private MathSiteDbContext Context { get; }
        private MathServices Services { get; }
        private DbSet<File> Repository { get; }
        private readonly IFileStorage _fileStorage;
        private IMapper Mapper { get; }

        public FileFacade(
            MathSiteDbContext context,
            MathServices services,
            IFileStorage fileStorage,
            IMapper mapper
        )
        {
            Context = context;
            Services = services;
            Repository = context.Set<File>();
            _fileStorage = fileStorage;
            Mapper = mapper;
        }
        public async Task<Guid> SaveFileAsync(string name, Stream data, Guid dirId)
        {
            // Removed for time it works
            //var hasRight = await Services.Users.HasCurrentUserRightAsync("admin");
            //if (!hasRight)
            //    throw new AuthenticationException("You must be authenticated and authorized for this action!");

            var hash = GetFileHashString(data);

            var alreadyExistsFile =
                await Repository.FirstOrDefaultAsync(f => f.Hash == hash);

            using (data)
            {
                if (data.CanSeek)
                    data.Seek(0, SeekOrigin.Begin);

                var pathId = alreadyExistsFile != null
                    ? alreadyExistsFile.Path
                    : await _fileStorage.SaveFileAsync(name, data);


                var file = new File
                {
                    Hash = hash,
                    Extension = Path.GetExtension(name),
                    Name = GetFileName(name, alreadyExistsFile),
                    Path = pathId,
                    DirectoryId = dirId != Guid.Empty
                        ? dirId
                        : null as Guid?
                };
                var entry = await Repository.AddAsync(file);
                await Context.SaveChangesAsync();

                return entry.Entity.Id;
            }
        }
        public async Task Remove(Guid id)
        {
            var file = await Repository
                .Include(f=>f.Person)
                .Include(f=>f.PostAttachments)
                .Include(f=>f.PostSettings)
                .FirstOrDefaultAsync(f=>f.Id==id);

            if (file.Person != null || file.PostAttachments.Count != 0 || file.PostSettings.Count != 0)
            {
                throw new FileIsUsedException();
            }

            var deleteFileTask = await Repository.CountAsync(f => f.Path == file.Path) <= 1
                ? _fileStorage.Remove(file.Path)
                : Task.CompletedTask;
            var entity = await Repository.FirstOrDefaultAsync(f2=>f2.Path == file.Path);
            Repository.Remove(entity);
            await Context.SaveChangesAsync();
        }
        private static string GetFileName(string currentName, File file)
        {
            if (file == null)
                return currentName;

            var splitedName = Path.GetFileNameWithoutExtension(file.Name)?.Split(new[] { "_" }, StringSplitOptions.None)
                .ToList();

            if (splitedName?.Count > 1 && long.TryParse(splitedName.LastOrDefault(), out var postfixValue))
                splitedName[splitedName.Count - 1] = $"{++postfixValue}";
            else
                splitedName?.Add("1");

            return splitedName?.Aggregate((f, s) => $"{f}_{s}") + Path.GetExtension(currentName);
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
