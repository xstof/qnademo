using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Qna.Backend.WireModels
{
    public class Session
    {
        public Session(){
            this.Questions = new List<Question>();
        }

        [JsonProperty("id")]
        public string Id { get;set;}

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        
        [JsonProperty("sessionName")]
        public string SessionName { get; set; }

        [JsonProperty("lastReleasedQuestionId")]
        public string LastReleasedQuestionId { get; set; }

        [JsonProperty("questions")]
        public List<Question> Questions { get; set; }
    }
}