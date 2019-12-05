using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Qna.Backend.CosmosModels
{
    public class SessionResults
    {
        public SessionResults(){
            this.Questions = new List<QuestionResults>();
        }

        [JsonProperty("type")]
        public string Type { get; set; } = "SessionResult";

        [JsonProperty("id")]
        public string Id { get;set;}

        [JsonProperty("partitionId")]
        public string PartitionId { get;set;}

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("sessionName")]
        
        public string SessionName { get; set; }
        [JsonProperty("questions")]
        public List<QuestionResults> Questions { get; set; } = new List<QuestionResults>();
    }

    public static class CosmosSessionResultsExtensions{
        public static Qna.Backend.WireModels.SessionResults ToWireModel(this SessionResults sessionResults){
            return new Qna.Backend.WireModels.SessionResults(){
                Id = sessionResults.Id,
                SessionId = sessionResults.SessionId,
                SessionName = sessionResults.SessionName,
                Questions = sessionResults.Questions.Select(q => q.ToWireModel()).ToList()
            };
        }
    }
}