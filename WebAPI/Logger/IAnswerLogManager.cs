using SurveyHeaven.Application.DTOs.Requests;

namespace SurveyHeaven.WebAPI.Logger
{
    public interface IAnswerLogManager : ILogManager<CreateAnswerRequest, UpdateAnswerRequest>
    {
        void BlankIpError(string controllerName, string actionName, CreateAnswerRequest request);
        void AlreadyUsedIpInformation(string controllerName, string actionName, CreateAnswerRequest request);
    }
}
