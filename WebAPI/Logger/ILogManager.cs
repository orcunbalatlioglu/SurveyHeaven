using SurveyHeaven.Application.DTOs;

namespace SurveyHeaven.WebAPI.Logger
{
    public interface ILogManager<TCreate, TUpdate> 
    where TCreate : class,IDto 
    where TUpdate : class,IDto
    {
        
        void NotExistInServer(string controllerName, string actionName, string id);
        void NotExistInServer(string controllerName, string actionName, TUpdate request);
        void NotExistInServer(string controllerName, string actionName);
        void SuccesfullEdit(string controllerName, string actionName, TUpdate request);
        void SuccesfullCreate(string controllerName, string actionName, TCreate request);
        void SuccesfullDelete(string controllerName, string actionName, string id);
        void SuccesfullGet(string controllerName, string actionName, string id);
        void SuccesfullGetAll(string controllerName, string actionName);
        void InvalidCreate(string controllerName, string actionName, TCreate request);
        void InvalidEdit(string controllerName, string actionName, TUpdate request);
        void BlankRequestId(string controllerName, string actionName, TUpdate request);
        void UnableDelete(string controllerName, string actionName, string id);
        void UnableCreate(string controllerName, string actionName, TCreate request);
        void ExceptionOccured(string controllerName, string actionName, TCreate request, string exception);
        void ExceptionOccured(string controllerName, string actionName, TUpdate request, string exception);
        void ExceptionOccured(string controllerName, string actionName, string id, string exception);
        void ExceptionOccured(string controllerName, string actionName, string exception);
    }
}
