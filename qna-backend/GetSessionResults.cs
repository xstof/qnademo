using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Qna.Backend.CosmosModels;
using Newtonsoft.Json;

namespace Qna.Backend
{
    public static class GetSessionResults
    {
        [FunctionName("GetSessionResults")]
        public static async Task<IActionResult> RunGetSessionResults(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sessions/{sessionId}/results")] HttpRequest req,
            string sessionId, 
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{sessionId}",
                Id = "sessionresults-{sessionId}")] CosmosModels.SessionResults sessionResults,
            ILogger log)
        {
            log.LogInformation("GetSessionResults was called.");

            if(String.IsNullOrWhiteSpace(sessionId)){
                log.LogError("Invalid session id provided");
                return new BadRequestObjectResult("Please provide a valid session id");
            }   
            else{
                if(sessionResults == null){
                    log.LogError("Session results not found for session: " + sessionId);
                    return new NotFoundObjectResult("Session not found: " + sessionId);
                }
                else{
                    log.LogInformation("Returned session with id: " + sessionId);

                    // return ok
                    return new OkObjectResult(
                        sessionResults.ToWireModel()
                    );
                }                
            }
        }
    }
}
