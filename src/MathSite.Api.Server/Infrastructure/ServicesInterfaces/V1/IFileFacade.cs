using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MathSite.Api.Server.Infrastructure.ServicesInterfaces.V1
{
    public interface IFileFacade
    {
        /// <summary></summary>
        /// <param name="currentUser"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="dirPath"></param>
        /// <exception cref="AuthenticationException">You must be authenticated and authorized for this action!</exception>
        /// <returns></returns>
        Task<Guid> SaveFileAsync(string name, Stream data, Guid directoryId);

        //Task<IEnumerable<File>> GetFilesByExtensions(IEnumerable<string> extensions);

        //Task<(string FileName, Stream FileStream, string Extension)> GetFileAsync(Guid id);
        Task Remove(Guid id);
    }
}
