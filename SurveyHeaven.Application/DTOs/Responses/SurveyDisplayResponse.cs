using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.Application.DTOs.Responses
{
    public class SurveyDisplayResponse : IDto
    {
        public string Id { get; set; }
        public string CreatedUserId { get; set; }
        public string? LastEditByUserId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
