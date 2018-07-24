using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class PpsxFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } =
            "application/vnd.openxmlformats-officedocument.presentationml.slideshow";

        public override IEnumerable<string> Extensions { get; } = new[] {".ppsx"};
    }
}