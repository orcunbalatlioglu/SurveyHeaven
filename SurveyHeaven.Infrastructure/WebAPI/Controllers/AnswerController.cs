using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using System.Net;

//TODO: Bütün işlemler için loglamaları yap.
//TODO: Kullanıcı ipsini post içinden almayı dene.

namespace WebAPI.Controllers
{
    
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AnswerController> _logger;

        public AnswerController(IAnswerService answerService,
                                IHttpContextAccessor httpContextAccessor,
                                ILogger<AnswerController> logger)
        {
            _answerService = answerService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateAnswerRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                string ipAddress = getClientIp();
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde ip adresine erişilememiştir.");
                    return Forbid("İsteğinizde ip adresine erişilememiştir.");
                }
                if (ModelState.IsValid)
                {
                    var isReplied = await checkIfRepliedBeforeBySameUser(request.SurveyId);
                    if (isReplied)
                    {
                        _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki ip adresi daha önce aynı anketi doldurduğu için tekrar cevaplamasına izin verilmemiştir.");
                        return BadRequest("Bu anket daha önce doldurulmuş!");
                    }
                    await _answerService.CreateAsync(request, ipAddress);
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucuda yeni bir varlık oluşturulmuştur.");
                    return Ok();
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için yeni bir varlık oluşturulamamıştır.");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateAnswerRequest request, string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                if (ModelState.IsValid)
                {                    
                    var isExist = await _answerService.IsExistsAsync(request.Id);
                    if (!isExist)
                    {
                        _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında sunucuda belirtilen id ile eşleşen bir varlık bulunamamıştır.");
                        return NotFound();
                    }

                    await _answerService.UpdateAsync(request);
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucudaki varlık düzenlenmiştir.");
                    return Ok();                    
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için varlık düzenlememiştir.");
                return BadRequest(ModelState);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try { 
                var isExist = await _answerService.IsExistsAsync(id);
                if (!isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                await _answerService.DeleteAsync(id);

                isExist = await _answerService.IsExistsAsync(id);
                if (isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık silinmeye çalışılmıştır fakat sunucudan başarılı şekilde silinememiştir.");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık başarılı bir şekilde sunucudan silinmiştir.");
                return Ok();
            }
            catch(Exception ex) 
            {
                
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetAnswer(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try { 
                var isExist = await _answerService.IsExistsAsync(id);
                if (!isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                var answer = await _answerService.GetByIdAsync(id);

                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
                return Ok(answer);
            }
            catch(Exception ex) 
            {                
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetSurveyAnswers")]
        public async Task<IActionResult> GetSurveyAnswers(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try {                 
                var answers = await _answerService.GetBySurveyIdAsync(id);
                if (answers.Count() > 0)
                {
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlıklar başarılı bir şekilde kullanıcıya iletilmiştir.");
                    return Ok(answers);
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetForEdit")]
        public async Task<IActionResult> GetAnswerForEdit(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var isExist = await _answerService.IsExistsAsync(id);
                if (!isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                var updateDisplay = await _answerService.GetForUpdateAsync(id);

                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
                return Ok(updateDisplay);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllAnswers()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try { 
                var allAnswers = await _answerService.GetAllAsync();
                if (allAnswers.Count() > 0)
                {
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde sunucudaki eşleşen bütün varlıklar başarılı bir şekilde kullanıcıya iletilmiştir.");
                    return Ok(allAnswers);
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde sunucuda eşleşen herhangi bir varlık bulunamamıştır.");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task<bool> checkIfRepliedBeforeBySameUser(string surveyId)
        {
            var ipAddress = getClientIp();
            var replies = await _answerService.GetForSameUserCheckBySurveyIdAsync(surveyId);
            foreach(var reply in replies)
            {
                if (reply.UserIp == ipAddress) 
                {
                    return true;
                }
            }
            return false;
        }

        private string? getClientIp()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
    }
}
