using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebUI.Models;
using WebUI.Models.DTOs.Requests;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService) 
        { 
            _surveyService = surveyService;
        }

        public async Task<IActionResult> ListUserSurveys()
        {
            try { 
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var response = await _surveyService.GetByUserIdAsync(userId);

                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var response = await _surveyService.GetForEditAsync(id);

                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(UpdateSurveyRequest request, string id)
        {
            try
            {
                request.Id = id;
                if(ModelState.IsValid) { 
                    await _surveyService.EditAsync(request);
                    
                    return RedirectToAction(nameof(ListUserSurveys));
                }
                return View(request);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }

        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {                
                await _surveyService.DeleteAsync(id);

                return RedirectToAction(nameof(ListUserSurveys));                
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }

        }

    }
}
