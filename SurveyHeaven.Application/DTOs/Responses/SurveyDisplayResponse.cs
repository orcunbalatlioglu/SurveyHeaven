using SurveyHeaven.Domain.Entities;

namespace SurveyHeaven.Application.DTOs.Responses
{
    public class SurveyDisplayResponse : IDto
    {
        public List<Question> Questions { get; set; }
    }
}
