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
    public static class GetSession
    {
        [FunctionName("GetSession")]
        public static async Task<IActionResult> RunGetSession(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sessions/{sessionId}")] HttpRequest req,
            string sessionId, 
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{sessionId}",
                Id = "{sessionId}")] CosmosModels.Session session,
            ILogger log)
        {
            log.LogInformation("GetSession was called.");

            if(String.IsNullOrWhiteSpace(sessionId)){
                log.LogError("Invalid session id provided");
                return new BadRequestObjectResult("Please provide a valid session id");
            }   
            else{
                if(session == null){
                    log.LogError("Session not found: " + sessionId);
                    return new NotFoundObjectResult("Session not found: " + sessionId);
                }
                else{
                    log.LogInformation("Returned session with id: " + sessionId);

                    // return ok
                    return new OkObjectResult(
                        session.ToWireModel()
                    );
                }                
            }
        }

        [FunctionName("GetSessionSummary")]
        public static async Task<IActionResult> RunGetSessionSummary(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sessions/{sessionId}/summary")] HttpRequest req,
            string sessionId, 
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{sessionId}",
                Id = "{sessionId}")] CosmosModels.Session session,
            ILogger log)
        {
            log.LogInformation("GetSessionSummary was called.");

            if(String.IsNullOrWhiteSpace(sessionId)){
                log.LogError("Invalid session id provided");
                return new BadRequestObjectResult("Please provide a valid session id");
            }   
            else{
                if(session == null){
                    log.LogError("Session not found: " + sessionId);
                    return new NotFoundObjectResult("Session not found: " + sessionId);
                }
                else{
                    log.LogInformation("Returned session summary with id: " + sessionId);

                    // return ok
                    return new OkObjectResult(
                        session.ToSummaryWireModel()
                    );
                }                
            }
        }
    }
}
