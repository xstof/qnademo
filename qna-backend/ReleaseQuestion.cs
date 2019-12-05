using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Qna.Backend
{
    public static class ReleaseQuestion
    {
        [FunctionName("ReleaseQuestion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/{sessionId}/questions/{questionId}/release")] HttpRequest req,
            string sessionId,
            string questionId,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{sessionId}",
                Id = "{sessionId}")] CosmosModels.Session session,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring")]IAsyncCollector<CosmosModels.Session> updatedSessions,
            ILogger log)
        {
            log.LogInformation("ReleaseQuestion was called.");

            if(session == null){
                log.LogError("Invalid session id provided");
                return new BadRequestObjectResult("Please provide a valid session id");
            }
            else if(!session.Questions.Any(q => q.Id == questionId)){
                log.LogError("Invalid question id provided");
                return new BadRequestObjectResult("Please provide valid question id");
            }
            var question = session.Questions.First(q => q.Id == questionId);
            if(question.IsReleased == true){
                log.LogError("Question was already released");
                return new BadRequestObjectResult("Question was already released");
            }
            
            // release question
            session.Questions.First(q => q.Id == questionId).IsReleased = true;
            session.LastReleasedQuestionId = questionId;
            await updatedSessions.AddAsync(session);

            log.LogInformation("Released question with id: " + questionId + "  on session: " + sessionId);

            // return ok
            return new OkResult();
        }
    }
}
