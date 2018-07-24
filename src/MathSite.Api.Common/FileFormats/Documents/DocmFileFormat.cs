using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class DocmFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/vnd.ms-word.document.macroEnabled.12";
        public override IEnumerable<string> Extensions { get; } = new[] {".docm"};
    }
}