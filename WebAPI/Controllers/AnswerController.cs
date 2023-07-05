using SurveyHeaven.WebAPI.Logger;
using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SurveyHeaven.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly ISurveyService _surveyService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAnswerLogManager _logManager;

        public AnswerController(IAnswerService answerService,
                                ISurveyService surveyService,
                                IHttpContextAccessor contextAccessor,
                                IAnswerLogManager logManager)
        {
            _answerService = answerService;
            _surveyService = surveyService;
            _contextAccessor = contextAccessor;
            _logManager = logManager;
        }

        [AllowAnonymous]
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
                    _logManager.BlankIpError(controllerName,actionName,request);
                    return Forbid("İsteğinizde ip adresine erişilememiştir.");
                }

                if (ModelState.IsValid)
                {
                    var isReplied = await checkIfRepliedBeforeBySameUser(request.SurveyId);
                    if (isReplied)
                    {                        
                        _logManager.AlreadyRepliedSurvey(controllerName,actionName,request);

                        string answerId = await _answerService.GetIdBySurveyIdAndUserIpAsync(request.SurveyId, ipAddress);
                        string message = "Bu anket daha önce doldurulmuş!";
                        return Conflict( new { message, answerId } );
                    }

                    var isSurveyExist = await _surveyService.IsExistsAsync(request.SurveyId);
                    if (isSurveyExist) { 
                        await _answerService.CreateAsync(request, ipAddress, getClientSignedInUserId());
                        _logManager.SuccesfullCreate(controllerName,actionName,request);
                        return Ok();
                    }
                    else
                    {
                        _logManager.InvalidCreate(controllerName,actionName,request);
                        return BadRequest("İstek içerisindeki surveyId ile eşleşen bir anket bulunamamıştır.");
                    }
                }

                _logManager.InvalidCreate(controllerName,actionName,request);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName,actionName,request,ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateAnswerRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            try
            {
                if (!string.IsNullOrWhiteSpace(request.Id)) 
                { 
                    if (ModelState.IsValid)
                    {                    
                        var isExist = await _answerService.IsExistsAsync(request.Id);
                        if (!isExist)
                        {
                            _logManager.NotExistInServer(controllerName,actionName,request);
                            return NotFound();
                        }

                        var clientIp = getClientIp();
                        var signedInUserId = getClientSignedInUserId();
                        var signedInRole = getClientSignedInRole();
                        Dictionary<string, string> userIdAndIp = await _answerService.GetIpAndUserIdByIdAsync(request.Id);
                        var answerIp = userIdAndIp["ip"];
                        var answerUserId = userIdAndIp["userId"];

                        if (answerIp == clientIp || (answerUserId == signedInUserId && signedInRole == "client") )
                        {
                            var unchangedAnswer = await _answerService.GetByIdAsync(request.Id);
                            await _answerService.UpdateAsync(request);
                            _logManager.SuccesfullEdit(controllerName, actionName, request);
                            return Ok();
                        }
                        else if(signedInRole == "editor" || signedInRole == "admin") 
                        {
                            await _answerService.UpdateAsync(request, signedInUserId);
                            _logManager.SuccesfullEdit(controllerName, actionName, request);
                            return Ok();
                        }
                        else { 
                            _logManager.UnauthorizedAccessTry(controllerName, actionName, signedInUserId, answerUserId);
                            return Forbid();
                        }
                    }

                    _logManager.InvalidEdit(controllerName,actionName,request);
                    return BadRequest(ModelState);
                }
                _logManager.BlankRequestId(controllerName,actionName,request);
                return BadRequest("Id boş!");
            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName,request,ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles ="admin,editor")]
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
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                await _answerService.DeleteAsync(id);

                isExist = await _answerService.IsExistsAsync(id);
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
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles = "editor,admin")]
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
                    _logManager.NotExistInServer(controllerName,actionName, id);
                    return NotFound();
                }
                var answer = await _answerService.GetByIdAsync(id);

                _logManager.SuccesfullGet(controllerName, actionName, id);
                return Ok(answer);
            }
            catch(Exception ex) 
            {                
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("GetSurveyAnswers")]
        public async Task<IActionResult> GetSurveyAnswers(string surveyId)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try {
                var role = getClientSignedInRole();
                bool isBelongToUser = await checkIsSurveyBelongToThisUser(surveyId);
                if (isBelongToUser || role == "admin" || role == "editor")
                {
                    var answers = await _answerService.GetBySurveyIdAsync(surveyId);
                    if (answers.Count() > 0)
                    {
                        _logManager.SuccesfullGet(controllerName, actionName, surveyId);
                        return Ok(answers);
                    }
                    _logManager.NotExistInServer(controllerName, actionName, surveyId);
                    return NotFound();
                }
                _logManager.UnauthorizedAccessTry(controllerName, actionName, getClientSignedInUserId(), surveyId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, surveyId, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
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
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }

                var clientIp = getClientIp();
                var signedInUserId = getClientSignedInUserId();
                var signedInRole = getClientSignedInRole();
                Dictionary<string, string> userIdAndIp = await _answerService.GetIpAndUserIdByIdAsync(id);
                var answerIp = userIdAndIp["ip"];
                var answerUserId = userIdAndIp["userId"];

                if ( clientIp == answerIp || (answerUserId == signedInUserId && signedInRole == "client"))
                {                    
                    var updateDisplay = await _answerService.GetForUpdateAsync(id);
                    _logManager.SuccesfullGet(controllerName, actionName, id);
                    return Ok(updateDisplay);
                }
                else if (signedInRole == "editor" || signedInRole == "admin")
                {
                    var updateDisplay = await _answerService.GetForUpdateAsync(id);
                    _logManager.SuccesfullGet(controllerName, actionName, id);
                    return Ok(updateDisplay);
                }
                else
                {
                    _logManager.UnauthorizedAccessTry(controllerName, actionName, signedInUserId, answerUserId);
                    return Forbid();
                }

            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName,actionName, id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles = "admin,editor")]
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
                    _logManager.SuccesfullGetAll(controllerName, actionName);
                    return Ok(allAnswers);
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

        private async Task<bool> checkIfRepliedBeforeBySameUser(string surveyId)
        {
            var ipAddress = getClientIp();
            var signedInUserId = getClientSignedInUserId();
            var signedInRole = getClientSignedInRole();
            var replies = await _answerService.GetForSameUserCheckBySurveyIdAsync(surveyId);

            foreach(var reply in replies)
            {
                if (reply.UserIp == ipAddress || (reply.UserId == signedInUserId && signedInRole == "client")) 
                {
                    return true;
                }
            }
            return false;
        }

        private string? getClientIp()
        {
            return _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
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
            foreach(var survey in surveys)
            {
                if (survey.Id == surveyId)
                    return true;
            }
            return false;
        }
    }
}
