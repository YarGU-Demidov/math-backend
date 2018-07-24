using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class PptxFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } =
            "application/vnd.openxmlformats-officedocument.presentationml.presentation";

        public override IEnumerable<string> Extensions { get; } = new[] {".pptx"};
    }
}