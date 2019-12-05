using System;
using Newtonsoft.Json;

namespace Qna.Backend.WireModels
{
    public partial class AnswerSubmission
    {   
        [JsonProperty("answerId")]
        public string AnswerId { get; set; }
    }
}