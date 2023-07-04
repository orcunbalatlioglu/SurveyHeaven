using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Enums;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.WebAPI.Logger;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

//TODO: Anketlere konulan soru ve seçenekler için filter yapısı getir.
//TODO: Anketlerdeki açık uçlu sorulara verilen cevaplar için filter yapısı getir.

namespace SurveyHeaven.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyLogManager _logManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public SurveyController(ISurveyService surveyService,
                                ISurveyLogManager logManager,
                                IHttpContextAccessor contextAccesor)
        {
            _surveyService = surveyService;
            _logManager = logManager;
            _contextAccessor = contextAccesor;
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
                    _logManager.InvalidQuestionType(controllerName, actionName, request);
                    return BadRequest("Geçersiz soru tipi. Soru tipi (checkbox,radio,text,textarea dışında bir şey olamaz!");
                }
                if (ModelState.IsValid)
                {
                    var signedInUserId = getClientSignedInUserId();

                    if (request.CreatedUserId == signedInUserId) { 
                        var id = await _surveyService.CreateAndReturnIdAsync(request);
                        bool isCreated = await _surveyService.IsExistsAsync(id);

                        if (isCreated)
                        {
                            _logManager.SuccesfullCreate(controllerName, actionName, request);
                            return Ok();
                        }

                        _logManager.UnableCreate(controllerName, actionName, request);                    
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                    else
                    {
                        _logManager.UnauthorizedAccessTry(controllerName, actionName, signedInUserId, request.CreatedUserId);
                        return Forbid();
                    }
                }
                _logManager.InvalidCreate(controllerName, actionName, request);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, request, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateSurveyRequest request)
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
                                _logManager.InvalidQuestionType(controllerName, actionName, request);
                                return BadRequest("Geçersiz soru tipi. Soru tipi checkbox, radio, text ve textarea dışında bir şey olamaz!");
                            }

                            var userRole = getClientSignedInRole();
                            bool isBelongToUser = await checkIsSurveyBelongToThisUser(request.Id);
                            if (userRole == "admin" || userRole == "editor" || (userRole == "client" && isBelongToUser) ) { 
                                await _surveyService.UpdateAsync(request);
                                _logManager.SuccesfullEdit(controllerName, actionName, request);
                                return Ok();
                            }
                            else
                            {
                                _logManager.UnauthorizedAccessTry(controllerName, actionName, getClientSignedInUserId(), request.Id);
                                return Forbid();
                            }
                        }
                        _logManager.InvalidEdit(controllerName, actionName, request);
                        return BadRequest(ModelState);
                    }
                    _logManager.NotExistInServer(controllerName, actionName,request);
                    return NotFound("Düzenlenmek istenen anket sunucuda bulunamadı!");
                }                
                _logManager.BlankRequestId(controllerName, actionName, request);
                return BadRequest("Id boş!");
            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, request, ex.Message);
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
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                var userRole = getClientSignedInRole();
                bool isBelongToUser = await checkIsSurveyBelongToThisUser(id);
                if (userRole == "admin" || userRole == "editor" || (userRole == "client" && isBelongToUser))
                {
                    await _surveyService.DeleteAsync(id.ToString());
                }
                else
                {
                    _logManager.UnauthorizedAccessTry(controllerName, actionName, getClientSignedInUserId(), id);
                    return Forbid();
                }

                isExist = await _surveyService.IsExistsAsync(id.ToString());
                if (isExist)
                {
                    _logManager.UnableDelete(controllerName, actionName, id);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                _logManager.SuccesfullDelete(controllerName, actionName, id);
                return Ok();
            }
            catch(Exception ex) 
            {
                _logManager.ExceptionOccured(controllerName,actionName,ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }        

        [AllowAnonymous]
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
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                _logManager.SuccesfullGet(controllerName,actionName, id);
                return Ok(surveyDisplay);
            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
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
                    var userRole = getClientSignedInRole();
                    bool isBelongToUser = await checkIsSurveyBelongToThisUser(id);
                    if (userRole == "admin" || userRole == "editor" || (userRole == "client" && isBelongToUser))
                    {
                        var updateDisplay = await _surveyService.GetForUpdateAsync(id.ToString());
                        _logManager.SuccesfullGet(controllerName, actionName, id);
                        return Ok(updateDisplay);
                    }
                    else
                    {
                        _logManager.UnauthorizedAccessTry(controllerName, actionName, getClientSignedInUserId(), id);
                        return Forbid();
                    }
                }
                _logManager.NotExistInServer(controllerName, actionName, id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
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
                var userRole = getClientSignedInRole();
                var signedUserId = getClientSignedInUserId();
                if (userRole == "admin" || userRole == "editor" || (userRole == "client" && signedUserId == id))
                {
                    var surveyDisplay = await _surveyService.GetByCreatedUserIdAsync(id.ToString());

                    if (surveyDisplay.Count() > 0)
                    {
                        _logManager.SuccesfullGet(controllerName, actionName, id);
                        return Ok(surveyDisplay);
                    }
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                else
                {
                    _logManager.UnauthorizedAccessTry(controllerName, actionName, signedUserId, id);
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles ="admin,editor")]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var userRole = getClientSignedInRole();
                var surveyDisplays = await _surveyService.GetAllAsync();

                if (surveyDisplays.Count() > 0)
                {
                    _logManager.SuccesfullGetAll(controllerName, actionName);
                    return Ok(surveyDisplays);
                }
                _logManager.NotExistInServer(controllerName, actionName);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, ex.Message);
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

        private string? getClientSignedInUserId()
        {
            return _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private string? getClientSignedInRole()
        {
            return _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
        }

        private async Task<bool> checkIsSurveyBelongToThisUser(string surveyId)
        {
            var userId = getClientSignedInUserId();
            var surveys = await _surveyService.GetByCreatedUserIdAsync(userId);
            foreach (var survey in surveys)
            {
                if (survey.Id == surveyId)
                    return true;
            }
            return false;
        }
    }
}
