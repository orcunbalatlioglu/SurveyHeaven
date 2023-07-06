using SurveyHeaven.Application.DTOs.Requests;

namespace SurveyHeaven.WebAPI.Logger
{
    public interface IUserLogManager : ILogManager<CreateUserRequest, UpdateUserRequest>
    {
        void InvalidUserRole(string controllerName, string actionName, CreateUserRequest request);
        void InvalidUserRole(string controllerName, string actionName, UpdateUserRequest request);
        void NotFoundUserLogin(string controllerName, string actionName, LoginRequest request);
        void SuccesfullUserLogin(string controllerName, string actionName, string id);
        void NotFoundSignedInUserInServer(string controllerName, string actionName, string signedInUserId);
    }
}
