using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs;

namespace WebAPI.Logger
{
    public abstract class BaseLogManager<T, TCreate, TUpdate, TDisplay> : ILogManager<TCreate, TUpdate>
    where TCreate : class, IDto
    where TUpdate : class, IDto
    where TDisplay : class, IDto
    {
        private readonly ILogger<T> _logger;

        public BaseLogManager(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void NotExistInServer(string controllerName, string actionName)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde sunucuda eşleşen herhangi bir varlık bulunamamıştır.");
        }

        public virtual void NotExistInServer(string controllerName, string actionName, string id)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde belirtilen {id} ile eşleşen bir varlık bulunamamıştır.");
        }

        public virtual void NotExistInServer(string controllerName, string actionName, TUpdate request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında sunucuda belirtilen id ile eşleşen bir varlık bulunamamıştır.");
        }

        public virtual void SuccesfullEdit(string controllerName, string actionName, TUpdate request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucudaki varlık düzenlenmiştir.");
        }

        public virtual void SuccesfullCreate(string controllerName, string actionName, TCreate request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucuda yeni bir varlık oluşturulmuştur.");
        }

        public void SuccesfullDelete(string controllerName, string actionName, string id)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık başarılı bir şekilde sunucudan silinmiştir.");
        }

        public void SuccesfullGet(string controllerName, string actionName, string id)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
        }

        public void SuccesfullGetAll(string controllerName, string actionName)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde sunucudaki eşleşen bütün varlıklar başarılı bir şekilde kullanıcıya iletilmiştir.");
        }

        public virtual void InvalidCreate(string controllerName, string actionName, TCreate request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için yeni bir varlık oluşturulamamıştır.");
        }

        public void InvalidEdit(string controllerName, string actionName, TUpdate request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için varlık düzenlememiştir.");
        }

        public void UnableDelete(string controllerName, string actionName, string id)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık silinmeye çalışılmıştır fakat belirlenemeyen bir sebepten dolayı sunucudan başarılı şekilde silinememiştir.");
        }

        public void UnableCreate(string controllerName, string actionName, TCreate request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında belirlemeyen bir sebepten dolayı yeni bir varlık oluşturulamamıştır.");
        }

        public virtual void ExceptionOccured(string controllerName, string actionName, TCreate request, string exception)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {exception}");
        }

        public virtual void ExceptionOccured(string controllerName, string actionName, TUpdate request, string exception)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {exception}");
        }

        public virtual void ExceptionOccured(string controllerName, string actionName, string id, string exception)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {exception}");
        }

        public void ExceptionOccured(string controllerName, string actionName, string exception)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {exception}");
        }

        public void BlankRequestId(string controllerName, string actionName, TUpdate request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {request} isteğinde id boş olduğu için işlem gerçekleştirilememiştir.");
        }
    }
}
