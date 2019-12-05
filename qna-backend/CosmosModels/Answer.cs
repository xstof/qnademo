using System;
using Newtonsoft.Json;

namespace Qna.Backend.CosmosModels
{
    public partial class Answer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionId")]
        public string PartitionId { get;set;}

        [JsonProperty("type")]
        public string Type { get; set; } = "Answer";

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("questionId")]
        public string QuestionId { get; set; }

        [JsonProperty("answerId")]
        public string AnswerId { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        public static Answer FromAnswerSubmission(string sessionId, string questionId, string userId, WireModels.AnswerSubmission submission){
            return new Answer(){
                Type = "Answer",
                Id = $"answer-{sessionId}-{questionId}-{userId}",
                PartitionId = $"session-{sessionId}",
                SessionId = sessionId,
                QuestionId = questionId,
                UserId = userId,
                AnswerId = submission.AnswerId
            };
        }
    }
}