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
using System.Linq;

namespace Qna.Backend
{
    public static class AddQuestion
    {
        [FunctionName("AddQuestion")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/{sessionId}/questions")] WireModels.Question questionNotSerializedDeepEnough,
            HttpRequest req, 
            string sessionId,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring",
                PartitionKey = "session-{sessionId}",
                Id = "{sessionId}")] CosmosModels.Session session,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring")] out CosmosModels.Session updatedSession,
            ILogger log)
        {
            log.LogInformation("AddQuestion was called.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var question = JsonConvert.DeserializeObject<WireModels.Question>(requestBody);

            if(string.IsNullOrWhiteSpace(question.Id)){
                log.LogError("Invalid question id provided");
                updatedSession = null;
                return new BadRequestObjectResult("Please provide a valid question id");
            }
            else if(question.AnswerOptions == null || 
               question.AnswerOptions.Count == 0 || 
               question.AnswerOptions.Any(o => string.IsNullOrWhiteSpace(o.Id))){
                   updatedSession = null;
                log.LogError("Invalid answer options provided with question");
                return new BadRequestObjectResult("Please provide valid answer options");
            }

            // throw is not exists yet
            if(session == null){
                updatedSession = null;
                return new BadRequestObjectResult("Session does not exist: " + sessionId);
            }

            // ensure question added is a new one
            if(session.Questions != null && session.Questions.Any(q => q.Id == question.Id)){
                updatedSession = null;
                log.LogError("Question with same id exists already: " + question.Id);
                return new BadRequestObjectResult("Question with same id exists already: " + question.Id);
            }

            // make sure a question starts out unreleased
            question.IsReleased = false;

            session.Questions.Add(Qna.Backend.CosmosModels.Question.FromWireModel(question));
            updatedSession = session;

            log.LogInformation("Added question to session with id: " + sessionId);

            // return ok
            return new OkResult();
        }
    }
}
