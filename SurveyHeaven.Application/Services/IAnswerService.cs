using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface IAnswerService : IService<CreateAnswerRequest, UpdateAnswerRequest, AnswerDisplayResponse>
    {
        Task CreateAsync(CreateAnswerRequest request, string ipAddress);
        void Update(UpdateAnswerRequest request, string userId, string userIp);
        Task UpdateAsync(UpdateAnswerRequest request, string userId, string userIp);
        IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId);
        Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId);
        IEnumerable<Answer> GetForSameUserCheckBySurveyId(string surveyId);
        Task<IEnumerable<Answer>> GetForSameUserCheckBySurveyIdAsync(string surveyId);
        Task<Dictionary<string, string>> GetIpAndUserIdByIdAsync(string id);
        Task<string> GetIdBySurveyIdAndUserIpAsync(string surveyId, string ipAddress);
    }
}
