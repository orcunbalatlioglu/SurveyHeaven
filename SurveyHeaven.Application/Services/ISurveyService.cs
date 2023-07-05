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
        void Update(UpdateSurveyRequest request, string signedInUserId);
        Task UpdateAsync(UpdateSurveyRequest request, string signedInUserId);
        IEnumerable<SurveyDisplayResponse> GetByCreatedUserId(string userId);
        Task<IEnumerable<SurveyDisplayResponse>> GetByCreatedUserIdAsync(string userId);
    }
}
