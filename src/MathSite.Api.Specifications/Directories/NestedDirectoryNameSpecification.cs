using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Directories
{
    public class NestedDirectoryNameSpecification : Specification<Directory>
    {
        private readonly Guid? _rootId;
        private readonly string _directoryName;

        public NestedDirectoryNameSpecification(Guid? rootId, string directoryName)
        {
            _rootId = rootId;
            _directoryName = directoryName;
        }

        public override Expression<Func<Directory, bool>> ToExpression()
        {
            return directory => directory.Name == _directoryName && directory.RootDirectoryId == _rootId;
        }
    }
}