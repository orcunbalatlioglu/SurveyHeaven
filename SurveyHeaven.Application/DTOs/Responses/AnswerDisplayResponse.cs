
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.Application.DTOs.Responses
{
    public class AnswerDisplayResponse : IDto
    {
        public List<Reply> Replies { get; set; }
    }
}
