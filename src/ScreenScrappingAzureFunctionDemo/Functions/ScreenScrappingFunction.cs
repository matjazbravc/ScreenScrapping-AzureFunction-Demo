using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using ScreenScrappingAzureFunctionDemo.Services;
using ScreenScrappingAzureFunctionDemo.Services.Ioc;
using ScreenScrappingAzureFunctionDemo.Services.Logging;

namespace ScreenScrappingAzureFunctionDemo.Functions
{
    public static class ScreenScrappingFunction
    {
        [FunctionName(nameof(ScreenScrappingFunction))]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, 
            [Inject] ILog log,
            [Inject] IScreenScrappingService screenScrappingService)
        {
            try
            {
                log.Info($"Starting {nameof(ScreenScrappingFunction)}");
                var result = screenScrappingService.GetResult(req);
                return result;
            }
            catch (Exception ex)
            {
                var exMessage = $"And error occured processing your request: {ex.Message}";
                log.Error(exMessage);
                return req.CreateResponse(HttpStatusCode.InternalServerError, exMessage);
            }
        }
    }
}
