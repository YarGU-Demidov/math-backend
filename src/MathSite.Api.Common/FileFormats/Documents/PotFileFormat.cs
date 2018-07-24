using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class PotFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/vnd.ms-powerpoint";
        public override IEnumerable<string> Extensions { get; } = new[] {".pot"};
    }
}