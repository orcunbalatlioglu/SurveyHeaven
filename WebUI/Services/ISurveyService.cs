using WebUI.Models.DTOs.Responses;
using WebUI.Models.DTOs.Requests;

namespace WebUI.Services
{
    public interface ISurveyService
    {
        Task<HttpResponseMessage> CreateAsync(CreateSurveyRequest request);
        Task<HttpResponseMessage> EditAsync(UpdateSurveyRequest request);
        Task<HttpResponseMessage> DeleteAsync(string id);
        Task<SurveyDisplayResponse> GetAsync(string id);
        Task<UpdateSurveyRequest> GetForEditAsync(string id);
        Task<IEnumerable<SurveyDisplayResponse>> GetByUserIdAsync(string userId);
        Task<IEnumerable<SurveyDisplayResponse>> GetAllAsync();
    }
}
