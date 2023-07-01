using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Enums;
using SurveyHeaven.Domain.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateSurveyRequest request)
        {
            if (!checkQuestionType(request.Questions))
                return BadRequest("Geçersiz soru tipi. Soru tipi (checkbox,radio,text,textarea dışında bir şey olamaz!");

            if (ModelState.IsValid)
            {
                var id = await _surveyService.CreateAndReturnIdAsync(request);
                bool isCreated = await _surveyService.IsExistsAsync(id);

                if (isCreated)
                {
                    return Ok();
                }
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            await _surveyService.DeleteAsync(id.ToString());

            bool isExist = await _surveyService.IsExistsAsync(id.ToString());
            if (isExist)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateSurveyRequest request, string id)
        {
            if (request != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    var isExist = await _surveyService.IsExistsAsync(request.Id);
                    if (isExist)
                    {
                        if (ModelState.IsValid)
                        {
                            if (!checkQuestionType(request.Questions))
                                return BadRequest("Geçersiz soru tipi. Soru tipi checkbox, radio, text ve textarea dışında bir şey olamaz!");

                            await _surveyService.UpdateAsync(request);
                            return Ok();
                        }
                        return BadRequest(ModelState);
                    }
                    return NotFound("Düzenlenmek istenen anket sunucuda bulunamadı!");
                }
            }
            return BadRequest("İstek boş!");
        }

        [HttpGet]
        [Route("GetSurveyForEdit")]
        public async Task<IActionResult> GetSurveyForEdit(string id)
        {
            bool isExist = await _surveyService.IsExistsAsync(id.ToString());
            if (isExist)
            {
                var updateDisplay = await _surveyService.GetForUpdateAsync(id.ToString());
                return Ok(updateDisplay);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("GetSurvey")]
        public async Task<IActionResult> GetSurvey(string id)
        {
            var surveyDisplay = await _surveyService.GetByIdAsync(id.ToString());

            if (surveyDisplay == null)
            {
                return NotFound();
            }

            return Ok(surveyDisplay);
        }

        [HttpGet]
        [Route("GetSurveyByUserId")]
        public async Task<IActionResult> GetSurveyByUserId(string id)
        {
            var surveyDisplay = await _surveyService.GetByCreatedUserIdAsync(id.ToString());

            if (surveyDisplay == null)
            {
                return NotFound();
            }

            return Ok(surveyDisplay);
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
