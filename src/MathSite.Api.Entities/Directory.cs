﻿using System;
using System.Collections.Generic;
using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    public class Directory : EntityWithName
    {
        public Guid? RootDirectoryId { get; set; }
        public Directory RootDirectory { get; set; }
        
        public ICollection<File> Files { get; set; } = new List<File>();
        public ICollection<Directory> Directories { get; set; } = new List<Directory>();
    }
}