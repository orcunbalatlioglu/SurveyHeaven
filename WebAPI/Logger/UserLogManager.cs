using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.WebAPI.Controllers;

namespace SurveyHeaven.WebAPI.Logger
{
    public class UserLogManager : BaseLogManager<UserController, CreateUserRequest, UpdateUserRequest, UserDisplayResponse>, IUserLogManager
    {
        private readonly ILogger<UserController> _logger;

        public UserLogManager(ILogger<UserController> logger) : base(logger)
        {
            _logger = logger;
        }

        public void InvalidUserRole(string controllerName, string actionName, CreateUserRequest request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki kullanıcı rolü geçersiz olduğundan yeni bir varlık oluşturulmasına izin verilmemiştir.");
        }

        public void InvalidUserRole(string controllerName, string actionName, UpdateUserRequest request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki kullanıcı rolü geçersiz olduğundan sunucudaki varlığın düzenlenmesine izin verilmemiştir.");
        }

        public void NotFoundUserLogin(string controllerName, string actionName, string email, string password)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki email:({email}) ve password:({password}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
        }

        public void SuccesfullUserLogin(string controllerName, string actionName, string id)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki bilgilerle id:{id} olan kullanıcı başarılı bir şekilde sisteme giriş yapmıştır.");
        }
    }
}
