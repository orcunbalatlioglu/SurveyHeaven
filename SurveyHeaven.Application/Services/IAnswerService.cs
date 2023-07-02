using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface IAnswerService : IService<CreateAnswerRequest, UpdateAnswerRequest, AnswerDisplayResponse>
    {
        Task CreateAsync(CreateAnswerRequest request, string ipAddress);
        void Update(UpdateAnswerRequest request);
        Task UpdateAsync(UpdateAnswerRequest request);
        IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId);
        Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId);
        IEnumerable<UpdateAnswerRequest> GetForSameUserCheckBySurveyId(string surveyId);
        Task<IEnumerable<Answer>> GetForSameUserCheckBySurveyIdAsync(string surveyId);
    }
}
