﻿using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class XltFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/vnd.ms-excel";
        public override IEnumerable<string> Extensions { get; } = new[] {".xlt"};
    }
}