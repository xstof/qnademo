using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Qna.Backend.CosmosModels;
using System.Linq;
using Microsoft.Azure.Documents.Client;

namespace Qna.Backend
{
    public static class ChangeFeedProcessor
    {
        [FunctionName("ChangeFeedProcessorGloballyRedundantFull")]
        public static async Task RunGlobalRedundantFullChangeFeed(
            [CosmosDBTrigger(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                LeaseCollectionPrefix = "%Region%",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true,
                ConnectionStringSetting = "cosmosconnectionstring")] IReadOnlyList<Document> input,
            [SignalR(HubName = "%Region%")]IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("ChangeFeedProcessorGloballyRedundantFull was called.");

            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
                foreach(Document inputdoc in input)
                {
                    // when session doc shows up in change feed: 
                    // - push full session to sessionUpdate() on authoring clients (groupname == $"authors-for-session-{sessionid}")
                    // - push summarized session to sessionUpdate() on participant clients (groupname == $"listeners-for-session-{sessionId}")

                    var docType = inputdoc.GetPropertyValue<string>("type").ToLower();
                    if(docType == "session"){
                        var session = JsonConvert.DeserializeObject<Session>(inputdoc.ToString());
                        var authorsGroupname = $"authors-for-session-{session.SessionId}";
                        var participantsGroupname = $"listeners-for-session-{session.SessionId}";

                        // Push into SignalR -> Authors
                        await signalRMessages.AddAsync(
                            new SignalRMessage{
                                Target = "sessionUpdate",
                                GroupName = authorsGroupname,
                                Arguments = new [] { session.ToWireModel() }
                            }
                        );

                        // Push into SignalR -> Participants
                        await signalRMessages.AddAsync(
                            new SignalRMessage{
                                Target = "sessionUpdateForParticipant",
                                GroupName = participantsGroupname,
                                Arguments = new [] { session.ToSummaryWireModel() }
                            }
                        );

                        log.LogInformation($"submitted signalr msg on target 'newQuestionReleased': {JsonConvert.SerializeObject(session.ToWireModel())}");
                    }
                    else if(docType == "sessionresult"){
                        var sessionResults = JsonConvert.DeserializeObject<SessionResults>(inputdoc.ToString());
                        // push updates to session result into signalr:
                        // Push into SignalR
                        // TODO: push into author channel only
                        var participantsGroupname = $"listeners-for-session-{sessionResults.SessionId}";
                        await signalRMessages.AddAsync(
                            new SignalRMessage{
                                Target = "sessionResultsUpdate",
                                GroupName = participantsGroupname,
                                Arguments = new [] { sessionResults }
                            }
                        );
                        log.LogInformation($"Send out signalr update for results of session: {sessionResults.SessionId}");
                    }
                };
            }           
        }

         class SessionResponseStats{
            public Dictionary<string, AnswerStats> AnswerStatsForQuestion { get; set; } = new Dictionary<string, AnswerStats>(); // key == questionId, provides answer stats for given question
        }
        class AnswerStats{
            public Dictionary<string, int> AnswerCounts {get; set; } = new Dictionary<string, int>(); // key == answerOptionId, provides count of responses for that answer
        }

        [FunctionName("ChangeFeedProcessorPartitioned")]
        public static async Task RunPartitionedChangeFeed(
            [CosmosDBTrigger(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                LeaseCollectionPrefix = "partitionedfeed",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true,
                ConnectionStringSetting = "cosmosconnectionstring",
                FeedPollDelay=1000,
                MaxItemsPerInvocation=100)
            ] IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "%cosmosdbname%",
                collectionName: "%cosmoscollectionname%",
                ConnectionStringSetting = "cosmosconnectionstring"
            )] DocumentClient client,
            [SignalR(HubName = "%Region%")]IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("ChangeFeedProcessorPartitioned was called.");

            string _databaseName = Environment.GetEnvironmentVariable("cosmosdbname");
            string _collectionName = Environment.GetEnvironmentVariable("cosmoscollectionname");
          
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                SessionResponseStats stats = new SessionResponseStats();
             
                var answerDocs = input.Where(doc => doc.GetPropertyValue<string>("type").ToLower() == "answer").ToList();
                log.LogInformation($"Found {answerDocs.Count} answer docs in the current changefeed processing iteration");
                var answers = answerDocs.Select(a => JsonConvert.DeserializeObject<Answer>(a.ToString())).ToList();

                var answersBySession = answers.GroupBy( a => a.SessionId );
                foreach(var sessionQuestions in answersBySession){
                    var sessionId = sessionQuestions.Key;
                    log.LogInformation($"Processing session: {sessionId}");

                    // fetch session from cosmos db
                    Document sessionDoc = null;
                    Session session = null;

                    var sessionDocPartitionKey = new PartitionKey($"session-{sessionId}");
                    var sessionDocId = $"{sessionId}";
                    var sessionDocUri = UriFactory.CreateDocumentUri(_databaseName, _collectionName, sessionDocId);

                    try{
                        sessionDoc = await client.ReadDocumentAsync(sessionDocUri, new RequestOptions(){ PartitionKey = sessionDocPartitionKey});
                        session = JsonConvert.DeserializeObject<Session>(sessionDoc.ToString());
                    }
                    catch(DocumentClientException ex){
                        log.LogError($"Error in retrieving session doc from cosmos for session {sessionId}.");
                        log.LogError(ex.Message);
                        continue; // session does not seem to exist or not retrievable, so just skip/drop the answer submissions for this session
                    }
                    
                    // fetch session results from cosmos db
                    Document sessionResultsdoc = null;
                    SessionResults sessionResults = null;

                    var sessionResultsDocPartitionKey = new PartitionKey($"session-{sessionId}");
                    var sessionResultsDocId = $"sessionresults-{sessionId}";
                    var sessionResultsDocUri = UriFactory.CreateDocumentUri(_databaseName, _collectionName, sessionResultsDocId);

                    try{
                        sessionResultsdoc = await client.ReadDocumentAsync(sessionResultsDocUri, new RequestOptions(){ PartitionKey = sessionResultsDocPartitionKey});
                        sessionResults = JsonConvert.DeserializeObject<SessionResults>(sessionResultsdoc.ToString());  
                    }
                    catch(DocumentClientException ex){
                        if(ex.StatusCode != System.Net.HttpStatusCode.NotFound){
                            log.LogError("Error in retrieving results doc from cosmos.");
                            log.LogError(ex.Message);
                            throw;
                        }
                    }
                    if(sessionResultsdoc == null) {
                        log.LogInformation($"  Results for session {sessionId} not found yet in cosmos db.  Need to create new results doc.");
                        sessionResultsdoc = new Document();
                        sessionResults = new SessionResults(){
                            Id = $"sessionresults-{session.SessionId}",
                            PartitionId = $"session-{session.SessionId}",
                            SessionId = session.SessionId,
                            SessionName = session.SessionName
                        };
                    }

                    // process all questions (and their answers) for this session:
                    log.LogInformation($"  Processing questions for session: {sessionId}");

                    var answersByQuestionId = sessionQuestions.GroupBy( s => s.QuestionId) ;
                    foreach(var questionAnswers in answersByQuestionId){
                        var questionId = questionAnswers.Key;
                        log.LogInformation($"    Processing answers for question {questionId} within session {sessionId}");
                        
                        if(session.Questions.Any(q => q.Id == questionId)){
                            var question = session.Questions.First(q => q.Id == questionId);
                            var questionResults = sessionResults.Questions.FirstOrDefault(q => q.QuestionId == questionId);
                            if(questionResults == null){
                                questionResults = new QuestionResults(){ QuestionId = questionId, Title = question.Title };
                                sessionResults.Questions.Add(questionResults);
                            }

                            // TODO: deduplicate multiple entries for the same answer on the same question by the same user:
                            // var answersByAnswerId = questionAnswers.GroupBy(a => a.AnswerId, a => new { AnswerId = a.AnswerId, QuestionId = a.QuestionId, UserId = a.UserId }, (answerId, answersByAllUsers)  => answersByAllUsers.Distinct());
                            var answersByAnswerId = questionAnswers.GroupBy(a => a.AnswerId);
                            foreach(var answersWithSameId in answersByAnswerId){
                                var answerId = answersWithSameId.Key;
                                log.LogInformation($"      Processing answers with answer id {answersWithSameId.Key} for question {questionId} within session {sessionId}");
                                var answerCount = answersWithSameId.Count();
                                log.LogInformation($"      Answercount is {answerCount} for answer with id {answersWithSameId.Key} for question {questionId} within session {sessionId}");
                                
                                // add votes to the votecount for this answer:
                                var votes = questionResults.Votes.FirstOrDefault(v => v.Id == answerId);
                                if(votes == null){
                                    votes = new VoteCount(){ Id = answerId, Votes = answerCount};
                                    questionResults.Votes.Add(votes);
                                }
                                else{
                                    votes.Votes += answerCount;
                                }
                            }

                        }

                    }
                
                    // write back updated session results to cosmos:
                    var collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
                    try{
                        var result = await client.UpsertDocumentAsync(collectionUri, sessionResults, new RequestOptions(){ PartitionKey = sessionDocPartitionKey });
                    }
                    catch(DocumentClientException ex){
                        log.LogError($"Failed to update session results for session {sessionId} in cosmos");
                        throw;
                    }
                }  
            } 
        }
    }
}
