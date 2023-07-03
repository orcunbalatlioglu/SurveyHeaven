using WebAPI.Logger;
using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using System.Net;


namespace WebAPI.Controllers
{
    
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAnswerLogManager _logManager;

        public AnswerController(IAnswerService answerService,
                                IHttpContextAccessor httpContextAccessor,
                                IAnswerLogManager logManager)
        {
            _answerService = answerService;
            _httpContextAccessor = httpContextAccessor;
            _logManager = logManager;
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
                    _logManager.BlankIpError(controllerName,actionName,request);
                    return Forbid("İsteğinizde ip adresine erişilememiştir.");
                }

                if (ModelState.IsValid)
                {
                    var isReplied = await checkIfRepliedBeforeBySameUser(request.SurveyId);
                    if (isReplied)
                    {
                        _logManager.AlreadyUsedIpInformation(controllerName,actionName,request);
                        return BadRequest("Bu anket daha önce doldurulmuş!");
                    }
                    await _answerService.CreateAsync(request, ipAddress);
                    _logManager.SuccesfullCreate(controllerName,actionName,request);
                    return Ok();
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

                        await _answerService.UpdateAsync(request);
                        _logManager.SuccesfullEdit(controllerName,actionName,request);
                        return Ok();                    
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
                    _logManager.SuccesfullGet(controllerName, actionName, id);
                    return Ok(answers);
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
                var updateDisplay = await _answerService.GetForUpdateAsync(id);

                _logManager.SuccesfullGet(controllerName, actionName, id);
                return Ok(updateDisplay);
            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName,actionName, id, ex.Message);
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
