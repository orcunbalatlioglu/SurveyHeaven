
using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.Application.DTOs.Responses
{
    public class AnswerDisplayResponse : IDto
    {
        public string SurveyId { get; set; }
        public string UserIp { get; set; }
        public string? UserId { get; set; }
        public string? LastEditByUserId { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
