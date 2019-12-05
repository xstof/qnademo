using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Qna.Backend.CosmosModels
{
    public class Question
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

        public static Question FromWireModel(WireModels.Question wireQuestion) {
            return new Question(){
                Title = wireQuestion.Title,
                Id = wireQuestion.Id,
                IsReleased = wireQuestion.IsReleased,
                AnswerOptions = wireQuestion.AnswerOptions.Select(opt => new AnswerOption(){ Id = opt.Id, Title = opt.Title }).ToList()
            };
        }   
    }
    public static class CosmosQuestionExtensions {
        public static Qna.Backend.WireModels.Question ToWireModel(this Question question){
            return new Qna.Backend.WireModels.Question(){
                Id = question.Id,
                IsReleased = question.IsReleased,
                Title = question.Title,
                AnswerOptions = question.AnswerOptions.Select(opt => new WireModels.AnswerOption() { Title = opt.Title, Id = opt.Id }).ToList()
            };
        }
    }
    
    public class AnswerOption
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }      
    }
}
