using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class MdbFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/vnd.ms-access";
        public override IEnumerable<string> Extensions { get; } = new[] {".mdb"};
    }
}