using System.Net.Http;

namespace ScreenScrappingAzureFunctionDemo.Services
{
    /// <summary>
    ///     This provides interfaces to the <see cref="ScreenScrappingService" /> class.
    /// </summary>
    public interface IScreenScrappingService
    {
        HttpResponseMessage GetResult(HttpRequestMessage req);
    }
}
