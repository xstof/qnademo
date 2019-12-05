using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Qna.Backend.WireModels
{
    public partial class Question
    {
        public Question(){
            this.AnswerOptions = new List<AnswerOption>();
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isReleased")]
        public bool IsReleased { get; set; }

        [JsonProperty("answerOptions")]
        public List<AnswerOption> AnswerOptions { get; set; }
    }

    public class AnswerOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }      
    }
}
