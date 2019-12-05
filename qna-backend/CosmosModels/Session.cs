using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Qna.Backend.CosmosModels
{
    public class Session
    {
        public Session(){
            this.Questions = new List<Question>();
        }

        [JsonProperty("type")]
        public string Type { get; set; } = "Session";

        [JsonProperty("id")]
        public string Id { get;set;}

        [JsonProperty("partitionId")]
        public string PartitionId { get;set;}

        [JsonProperty("authorSub")]
        public string AuthorSub { get;set;}

        [JsonProperty("authorEmail")]
        public string AuthorEmail { get;set;} 
        
        [JsonProperty("authorName")]
        public string AuthorName { get;set;}
        
        [JsonProperty("lastReleasedQuestionId")]
        public string LastReleasedQuestionId { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        [JsonProperty("sessionName")]
        
        public string SessionName { get; set; }
        [JsonProperty("questions")]
        public List<Question> Questions { get; set; } = new List<Question>();

        public static Session FromWireModel(WireModels.Session wireSession){
            return new Session(){
                Id = wireSession.Id,
                PartitionId = $"session-{wireSession.Id}",
                SessionId = wireSession.SessionId,
                SessionName = wireSession.SessionName,
                Questions = wireSession.Questions.Select(q => Question.FromWireModel(q)).ToList()
            };
        }
    }

    public static class CosmosSessionExtensions{
        public static Qna.Backend.WireModels.Session ToWireModel(this Session session){
            return new Qna.Backend.WireModels.Session(){
                Id = session.Id,
                SessionId = session.SessionId,
                SessionName = session.SessionName,
                LastReleasedQuestionId = session.LastReleasedQuestionId,
                Questions = session.Questions.Select(q => q.ToWireModel()).ToList()
            };
        }

        public static Qna.Backend.WireModels.SessionSummary ToSummaryWireModel(this Session session){
            WireModels.Question lastReleasedQuestion = null;
            if(session.Questions != null){
                var lastQuestion = session.Questions.FirstOrDefault(q => q.Id == session.LastReleasedQuestionId);
                if(lastQuestion != null){
                    lastReleasedQuestion = lastQuestion.ToWireModel();
                }
            }
            return new Qna.Backend.WireModels.SessionSummary(){
                SessionId = session.SessionId,
                SessionName = session.SessionName,
                LastReleasedQuestion = lastReleasedQuestion
            };
        }
    }
}