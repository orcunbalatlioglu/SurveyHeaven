using WebUI.Models.Entities;

namespace WebUI.Models.DTOs.Responses
{
    public class AnswerDisplayResponse
    {
        public string SurveyId { get; set; }
        public string UserIp { get; set; }
        public string? UserId { get; set; }
        public string? LastEditByUserId { get; set; }
        public List<Reply> Replies { get; set; }
    }
}
