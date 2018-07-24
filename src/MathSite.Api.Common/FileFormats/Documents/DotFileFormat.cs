using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class DotFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/msword";
        public override IEnumerable<string> Extensions { get; } = new[] {".dot"};
    }
}