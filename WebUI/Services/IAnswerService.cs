using WebUI.Models.DTOs.Responses;
using WebUI.Models.DTOs.Requests;

namespace WebUI.Services
{
    public interface IAnswerService
    {
        Task<HttpResponseMessage> CreateAsync(CreateAnswerRequest request);
        Task<HttpResponseMessage> EditAsync(UpdateAnswerRequest request);
        Task<HttpResponseMessage> DeleteAsync(string id);
        Task<AnswerDisplayResponse> GetAsync(string id);
        Task<UpdateAnswerRequest> GetForEditAsync(string id);
        Task<IEnumerable<AnswerDisplayResponse>> GetAllAsync();
        Task<IEnumerable<AnswerDisplayResponse>> GetSurveyAnswers(string surveyId);
    }
}
