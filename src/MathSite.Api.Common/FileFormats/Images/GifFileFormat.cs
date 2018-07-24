using System.Collections.Generic;

namespace MathSite.Api.Common.FileFormats.Images
{
    public class GifFileFormat : BaseFileFormat
    {
        public override string ContentType { get; } = "image/gif";
        public override IEnumerable<string> Extensions { get; } = new[] {".gif"};
        public override FileType FileType { get; } = FileType.Image;
    }
}