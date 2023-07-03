using SurveyHeaven.Application.DTOs.Requests;

namespace SurveyHeaven.WebAPI.Logger
{
    public interface IUserLogManager : ILogManager<CreateUserRequest, UpdateUserRequest>
    {
        public void InvalidUserRole(string controllerName, string actionName, CreateUserRequest request);
        public void InvalidUserRole(string controllerName, string actionName, UpdateUserRequest request);
        public void NotFoundUserLogin(string controllerName, string actionName, string email, string password);
        public void SuccesfullUserLogin(string controllerName, string actionName, string id);
        public void UnauthorizedProfileEdit(string controllerName, string actionName, string signedInUserId, string requestUserId);
        public void UnauthorizedProfileGetForUpdate(string controllerName, string actionName, string signedInUserId, string requestUserId);
    }
}
