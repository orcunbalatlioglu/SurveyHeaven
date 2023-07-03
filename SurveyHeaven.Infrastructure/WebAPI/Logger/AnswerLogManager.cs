﻿using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.DTOs.Responses;
using WebAPI.Controllers;

namespace WebAPI.Logger
{
    public class AnswerLogManager : BaseLogManager<AnswerController,CreateAnswerRequest,UpdateAnswerRequest,AnswerDisplayResponse>, IAnswerLogManager
    {
        private readonly ILogger<AnswerController> _logger;

        public AnswerLogManager(ILogger<AnswerController> logger) : base(logger) 
        { 
            _logger = logger;        
        }

        public void BlankIpError(string controllerName, string actionName, CreateAnswerRequest request)
        {
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde ip adresine erişilememiştir.");
        }

        public void AlreadyUsedIpInformation(string controllerName, string actionName, CreateAnswerRequest request)
        {
            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki ip adresi daha önce aynı anketi doldurduğu için tekrar cevaplamasına izin verilmemiştir.");
        }
    }
}
