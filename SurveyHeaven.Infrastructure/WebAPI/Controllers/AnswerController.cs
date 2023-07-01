using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;

//TODO: Bütün işlemler için loglamaları yap.

namespace WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateAnswerRequest request)
        {
            if(ModelState.IsValid)
            {
                var isReplied = await checkIfRepliedBeforeBySameUser(request.SurveyId,request.UserIp);
                if (isReplied)
                {
                    return BadRequest("Bu anket daha önce doldurulmuş!");
                }     
                await _answerService.CreateAsync(request);
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateAnswerRequest request, string id)
        {
            if (ModelState.IsValid)
            {
                var isExist = await _answerService.IsExistsAsync(request.Id);
                if (!isExist)
                {
                    return NotFound();
                }
                await _answerService.UpdateAsync(request);
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {           
            var isExist = await _answerService.IsExistsAsync(id);
            if (!isExist)
            {
                return NotFound();
            }
            await _answerService.DeleteAsync(id);

            isExist = await _answerService.IsExistsAsync(id);
            if (isExist)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> GetAnswer(string id)
        {           
            var answer = await _answerService.GetByIdAsync(id);
            if (answer is null)
            {
                return NotFound();
            }
            return Ok(answer);
        }

        [HttpGet]
        [Route("GetSurveyAnswers")]
        public async Task<IActionResult> GetSurveyAnswers(string id)
        {
            var answers = await _answerService.GetBySurveyIdAsync(id);
            if (answers.Count() > 0)
            {
                return Ok(answers);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetForUpdate")]
        public async Task<IActionResult> GetAnswerForUpdate(string id)
        {
            var updateDisplay = await _answerService.GetForUpdateAsync(id);
            if (updateDisplay is not null)
            {
                return Ok(updateDisplay);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllAnswers()
        {
            var allAnswers = await _answerService.GetAllAsync();
            if (allAnswers.Count() > 0)
            {
                return Ok(allAnswers);
            }
            return NotFound();
        }

        private async Task<bool> checkIfRepliedBeforeBySameUser(string surveyId, string ipAddress)
        {
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
    }
}
