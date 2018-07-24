using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Documents
{
    public class XlsbFileFormat : VendorFileFormatBase
    {
        public override string ContentType { get; } = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";

        public override IEnumerable<string> Extensions { get; } = new[] {".xlsb"};
    }
}