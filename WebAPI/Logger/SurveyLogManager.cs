using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using SurveyHeaven.WebAPI.Controllers;

namespace SurveyHeaven.WebAPI.Logger
{
    public class SurveyLogManager : BaseLogManager<SurveyController, CreateSurveyRequest, UpdateSurveyRequest, SurveyDisplayResponse>, ISurveyLogManager
    {
        private readonly ILogger<SurveyController> _logger;
        public SurveyLogManager(ILogger<SurveyController> logger): base(logger)
        {
            _logger = logger;
        }

        public void InvalidQuestionType(string controllerName, string actionName, CreateSurveyRequest request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki soru tipleri uygun olmadığı için anketin yaratılmasına izin verilmemiştir.");
        }

        public void InvalidQuestionType(string controllerName, string actionName, UpdateSurveyRequest request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde girilen soru tiplerinde geçersiz soru tipi bulunmasından dolayı varlık güncellenememiştir.");
        }
    }
}
