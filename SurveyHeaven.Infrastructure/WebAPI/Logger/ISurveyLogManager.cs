using SurveyHeaven.Application.DTOs.Requests;

namespace WebAPI.Logger
{
    public interface ISurveyLogManager : ILogManager<CreateSurveyRequest, UpdateSurveyRequest>
    {
        void InvalidQuestionType(string controllerName, string actionName, CreateSurveyRequest request);
        void InvalidQuestionType(string controllerName, string actionName, UpdateSurveyRequest request);
    }
}
