using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace Qna.Backend.WireModels
{
    public class SessionSummary
    {
        public SessionSummary(){}

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
        
        [JsonProperty("sessionName")]
        public string SessionName { get; set; }

        [JsonProperty("lastReleasedQuestion")]
        public Question LastReleasedQuestion { get; set; }
    }
}