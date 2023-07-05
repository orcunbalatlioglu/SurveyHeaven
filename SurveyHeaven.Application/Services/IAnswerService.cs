using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface IAnswerService : IService<CreateAnswerRequest, UpdateAnswerRequest, AnswerDisplayResponse>
    {
        void Create(CreateAnswerRequest request, string ipAddress, string userId);
        Task CreateAsync(CreateAnswerRequest request, string ipAddress, string userId);
        void Update(UpdateAnswerRequest request);
        Task UpdateAsync(UpdateAnswerRequest request);
        void Update(UpdateAnswerRequest request, string editByUserId);
        Task UpdateAsync(UpdateAnswerRequest request, string editByUserId);
        IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId);
        Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId);
        IEnumerable<Answer> GetForSameUserCheckBySurveyId(string surveyId);
        Task<IEnumerable<Answer>> GetForSameUserCheckBySurveyIdAsync(string surveyId);
        Task<Dictionary<string, string>> GetIpAndUserIdByIdAsync(string id);
        Task<string> GetIdBySurveyIdAndUserIpAsync(string surveyId, string ipAddress);
    }
}
