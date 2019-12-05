using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qna.Backend.WireModels;

namespace Qna.Backend
{
    public static class GetConfiguration
    {
        [FunctionName("GetConfiguration")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "configuration")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetConfiguration was called");

            var browserSessionId = System.Guid.NewGuid().ToString();
            var region = System.Environment.GetEnvironmentVariable("Region", EnvironmentVariableTarget.Process);
            var signalRNegotiateBaseUrl = System.Environment.GetEnvironmentVariable("SignalRNegotiateBaseUrl", EnvironmentVariableTarget.Process);
            var authority = System.Environment.GetEnvironmentVariable("AadB2CIssuer", EnvironmentVariableTarget.Process);
            var clientid = System.Environment.GetEnvironmentVariable("AadClientId", EnvironmentVariableTarget.Process);

            var config = new Configuration(){Region = region, 
                                             SignalRNegotiateUrl = signalRNegotiateBaseUrl,
                                             BrowserSessionId = browserSessionId };
            config.Auth = new Authentication(){
                Authority = authority,
                ClientId = clientid
            };                                 
            
            return new OkObjectResult(config);
        }
    }
}
