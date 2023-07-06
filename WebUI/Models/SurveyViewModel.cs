using WebUI.Models.DTOs.Requests;
using WebUI.Models.DTOs.Responses;

namespace WebUI.Models
{
    public class SurveyViewModel
    {
        public SurveyDisplayResponse Survey { get; set; }
        public CreateAnswerRequest Answer { get; set; }
    }
}
