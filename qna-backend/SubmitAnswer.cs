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

namespace Qna.Backend
{
    public static class SubmitAnswer
    {
        [FunctionName("SubmitAnswer")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/{sessionId}/questions/{questionid}/answers/{userId}")] WireModels.AnswerSubmission answerSubmission,
            HttpRequest req, 
            string sessionId,
            string questionId,
            string userId,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring")] out CosmosModels.Answer answerToInsert,
            ILogger log)
        {
            log.LogInformation("SubmitAnswer was called.");

            if(answerSubmission == null){
                log.LogError("Answer submitted was null");
                answerToInsert = null;
                return new BadRequestObjectResult("Please provide a valid answer object");
            }
            else if (String.IsNullOrWhiteSpace(answerSubmission.AnswerId)){
                log.LogError("AnswerId is invalid");
                answerToInsert = null;
                return new BadRequestObjectResult("Please provide a valid answerid");
            } 

            answerToInsert = CosmosModels.Answer.FromAnswerSubmission(sessionId, questionId, userId, answerSubmission);

            log.LogInformation($"Submitted answer on session {sessionId} for question with id {questionId} for user {userId}");

            // return ok
            return new OkResult();
        }
    }
}
