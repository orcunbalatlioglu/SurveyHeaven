using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Enums;
using SurveyHeaven.Domain.Entities;
using Amazon.Runtime.Internal;

//TODO: Anketlere konulan soru ve seçenekler için filter yapısı getir.
//TODO: Anketlerdeki açık uçlu sorulara verilen cevaplar için filter yapısı getir.

namespace WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService,
                                ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateSurveyRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                if (!checkQuestionType(request.Questions))
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki soru tipleri uygun olmadığı için anketin yaratılmasına izin verilmemiştir.");
                    return BadRequest("Geçersiz soru tipi. Soru tipi (checkbox,radio,text,textarea dışında bir şey olamaz!");
                }
                if (ModelState.IsValid)
                {
                    var id = await _surveyService.CreateAndReturnIdAsync(request);
                    bool isCreated = await _surveyService.IsExistsAsync(id);

                    if (isCreated)
                    {
                        _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucuda yeni bir varlık oluşturulmuştur.");
                        return Ok();
                    }
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında belirlemeyen bir sebepten dolayı yeni bir varlık oluşturulamamıştır.");
                    return StatusCode(StatusCodes.Status500InternalServerError);

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
        public async Task<IActionResult> Edit(UpdateSurveyRequest request, string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {                
                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    var isExist = await _surveyService.IsExistsAsync(request.Id);
                    if (isExist)
                    {
                        if (ModelState.IsValid)
                        {
                            if (!checkQuestionType(request.Questions)) {
                                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde girilen soru tiplerinde geçersiz soru tipi bulunmasından dolayı varlık güncellenememiştir.");
                                return BadRequest("Geçersiz soru tipi. Soru tipi checkbox, radio, text ve textarea dışında bir şey olamaz!");
                            }
                            
                            await _surveyService.UpdateAsync(request);
                            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucudaki varlık düzenlenmiştir.");
                            return Ok();
                        }
                        _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için yeni bir varlık oluşturulamamıştır.");
                        return BadRequest(ModelState);
                    }
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında sunucuda belirtilen id ile eşleşen bir varlık bulunamamıştır.");
                    return NotFound("Düzenlenmek istenen anket sunucuda bulunamadı!");
                }                
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde id boş olduğu için işlem gerçekleştirilememiştir.");
                return BadRequest("Id boş!");
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
            try
            {
                bool isExist = await _surveyService.IsExistsAsync(id.ToString());
                if (!isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde sunucuda belirtilen id ile eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                await _surveyService.DeleteAsync(id.ToString());

                isExist = await _surveyService.IsExistsAsync(id.ToString());
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
        [Route("GetForEdit")]
        public async Task<IActionResult> GetSurveyForEdit(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                bool isExist = await _surveyService.IsExistsAsync(id.ToString());
                if (isExist)
                {
                    var updateDisplay = await _surveyService.GetForUpdateAsync(id.ToString());
                    return Ok(updateDisplay);
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                return NotFound();
            }
            catch(Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetSurvey(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var surveyDisplay = await _surveyService.GetByIdAsync(id.ToString());

                if (surveyDisplay == null)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
                return Ok(surveyDisplay);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetByUserId")]
        public async Task<IActionResult> GetSurveyByUserId(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var surveyDisplay = await _surveyService.GetByCreatedUserIdAsync(id.ToString());

                if (surveyDisplay.Count() > 0)
                {
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlıklar başarılı bir şekilde kullanıcıya iletilmiştir.");
                    return Ok(surveyDisplay);                   
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

        private bool checkQuestionType(List<Question> Questions)
        {
            foreach (var question in Questions)
            {
                if (question.Type != QuestionType.Radio && question.Type != QuestionType.Checkbox && question.Type != QuestionType.Text && question.Type != QuestionType.TextArea)
                    return false;
            }
            return true;
        }
    }
}
