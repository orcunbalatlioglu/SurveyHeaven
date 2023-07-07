using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using WebUI.Models.Entities;
using WebUI.Models.DTOs.Requests;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly ISurveyService _surveyService;

        public AnswerController(IAnswerService answerService,
                                ISurveyService surveyService) 
        { 
            _answerService = answerService;
            _surveyService = surveyService;
        }

        public async Task<IActionResult> Create(string id)
        {
            try
            {
                var surveyDisplay = await _surveyService.GetAsync(id);
                List<Reply> replyList = new List<Reply>();
                foreach (var question in surveyDisplay.Questions)
                {
                    replyList.Add(new Reply() { QuestionId = question.Id });
                }

                SurveyViewModel model = new SurveyViewModel
                {
                    Survey = surveyDisplay,
                    Answer = new CreateAnswerRequest()
                    {
                        SurveyId = id,
                        Replies = replyList
                    }
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAnswerRequest request)
        {
            if(ModelState.IsValid)
            {
                try 
                { 
                    var response = await _answerService.CreateAsync(request);
                    return RedirectToAction(nameof(SuccesfullCreate));   
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
                }
            }

            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var answer = await _answerService.GetForEditAsync(id);
            return View(answer);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(UpdateAnswerRequest request)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _answerService.EditAsync(request);
                    return RedirectToAction(nameof(SuccesfullEdit));
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
                }
            }

            return View();
        }

        public async Task<IActionResult> Delete(string id)
        {            
            try
            {
                var response = await _answerService.DeleteAsync(id);
                return RedirectToAction(nameof(SuccesfullDelete));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var response = await _answerService.GetAsync(id);
                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> ListAll()
        {
            try
            {
                var response = await _answerService.GetAllAsync();
                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> ListSurveyAnswers(string surveyId)
        {
            try
            {
                var response = await _answerService.GetSurveyAnswers(surveyId);
                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> SuccesfullCreate()
        {
            return View();
        }

        public async Task<IActionResult> SuccesfullEdit()
        {
            return View();
        }

        public async Task<IActionResult> SuccesfullDelete()
        {
            return View();
        }

        
    }
}
