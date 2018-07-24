using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Directories
{
    public class DirectoryIdSpecification : Specification<Directory>
    {
        private readonly Guid _dirId;

        public DirectoryIdSpecification(Directory directory)
        {
            _dirId = directory.Id;
        }
        public DirectoryIdSpecification(Guid directoryId)
        {
            _dirId = directoryId;
        }

        public override Expression<Func<Directory, bool>> ToExpression()
        {
            return directory => directory.Id == _dirId;
        }
    }
}