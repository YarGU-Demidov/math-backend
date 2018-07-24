﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MathSite.Api.Common.Specifications;
using MathSite.Api.Entities;

namespace MathSite.Api.Specifications.Files
{
    public class FileExtensionsSpecification : Specification<File>
    {
        private readonly IEnumerable<string> _extensions;

        public FileExtensionsSpecification(IEnumerable<string> extensions)
        {
            _extensions = extensions;
        }

        public FileExtensionsSpecification(string extension)
            : this(new[] {extension})
        {
        }

        public override Expression<Func<File, bool>> ToExpression()
        {
            return file => _extensions.Any(ext => ext == file.Extension);
        }
    }
}