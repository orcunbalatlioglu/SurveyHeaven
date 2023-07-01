using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface ISurveyService : IService<CreateSurveyRequest, UpdateSurveyRequest, SurveyDisplayResponse>
    {
        string CreateAndReturnId(CreateSurveyRequest request);
        Task<string> CreateAndReturnIdAsync(CreateSurveyRequest request);
        void Update(UpdateSurveyRequest request);
        Task UpdateAsync(UpdateSurveyRequest request);
        IEnumerable<UpdateSurveyRequest> GetByCreatedUserId(string userId);
        Task<IEnumerable<UpdateSurveyRequest>> GetByCreatedUserIdAsync(string userId);
    }
}
