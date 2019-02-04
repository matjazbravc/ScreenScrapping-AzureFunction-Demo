using System.ComponentModel;

namespace ScreenScrappingAzureFunctionDemo.Services.Enums
{
    public enum EncodingType
    {
        [Description("gzip")]
        Gzip,
        [Description("deflate")]
        Deflate
    }
}
