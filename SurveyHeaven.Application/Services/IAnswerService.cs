using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;

namespace SurveyHeaven.Application.Services
{
    public interface IAnswerService : IService<CreateAnswerRequest, UpdateAnswerRequest, AnswerDisplayResponse>
    {
        void Update(UpdateAnswerRequest request);
        Task UpdateAsync(UpdateAnswerRequest request);
        IEnumerable<AnswerDisplayResponse> GetBySurveyId(string surveyId);
        Task<IEnumerable<AnswerDisplayResponse>> GetBySurveyIdAsync(string surveyId);
        IEnumerable<UpdateAnswerRequest> GetForSameUserCheckBySurveyId(string surveyId);
        Task<IEnumerable<UpdateAnswerRequest>> GetForSameUserCheckBySurveyIdAsync(string surveyId);
    }
}
