using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using Microsoft.Azure;
using ScreenScrappingAzureFunctionDemo.Services.Logging;
using Serilog;

namespace ScreenScrappingAzureFunctionDemo.Services
{
    /// <summary>
    ///     This represents the function that screen scrap certain web page.
    /// </summary>
    public class ScreenScrappingService : IScreenScrappingService
    {
        private readonly ILog _log;
        private readonly HttpRequestMessageHelper _httpRequestMessageHelper;

        public ScreenScrappingService(ILog log, HttpRequestMessageHelper httpRequestMessageHelper)
        {
            _log = log;
            _log.Debug();
            _httpRequestMessageHelper = httpRequestMessageHelper;
        }

        /// <summary>
        ///     Invokes the function.
        /// </summary>
        /// <param name="req"><see cref="HttpRequestMessage" /> instance.</param>
        /// <returns>Returns the <see cref="HttpResponseMessage" /> instance.</returns>
        public HttpResponseMessage GetResult(HttpRequestMessage req)
        {
            HttpResponseMessage responseMessage;
            try
            {
                _log.Info("Scrapping titles has started");

                var urlAddress = CloudConfigurationManager.GetSetting("Url.Address");
                if (string.IsNullOrWhiteSpace(urlAddress))
                {
                    return _httpRequestMessageHelper.CreateBadRequestResponse(req, "Null or empty UrlAddress");
                }

                // Decompress response
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                     DecompressionMethods.Deflate;
                }

                string html;
                using (var client = new HttpClient(handler))
                {
                    html = client.GetStringAsync(urlAddress).Result;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var postTitles = doc.DocumentNode
                    .Descendants("td")
                    .Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains("title"))
                    .Select(x => x.InnerText).ToList();

                responseMessage = _httpRequestMessageHelper.CreateOkResponse(req, postTitles);

                _log.Info($"Scrapping {postTitles.Count} titles has ended");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                responseMessage = req.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

            return responseMessage;
        }
    }
}
