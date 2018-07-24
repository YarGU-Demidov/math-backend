using System;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Directories
{
    public class DirectoryNameSpecification : Specification<Directory>
    {
        private readonly string _directoryName;

        public DirectoryNameSpecification(string directoryName)
        {
            _directoryName = directoryName;
        }

        public override Expression<Func<Directory, bool>> ToExpression()
        {
            return directory => directory.Name == _directoryName;
        }
    }
}