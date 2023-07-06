using WebUI.Models.Entities;

namespace WebUI.Models.DTOs.Responses
{
    public class SurveyDisplayResponse
    {
        public string Id { get; set; }
        public string CreatedUserId { get; set; }
        public string? LastEditByUserId { get; set; }
        public List<Question> Questions { get; set; }
    }
}
