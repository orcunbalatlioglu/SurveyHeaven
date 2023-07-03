using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Enums;
using SurveyHeaven.WebAPI.Logger;

//TODO: Kullanıcı girişinde JWT işlemlerini yap.
//TODO: İşlemelere role based authorize getir.
//TODO: Hangfire'ı bütün projede kullanmaya çalış.

namespace SurveyHeaven.WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserLogManager _logManager;

        public UserController(IUserService userService,
                              IUserLogManager logManager)
        {
            _userService = userService;
            _logManager = logManager;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (ModelState.IsValid)
            {
                try { 
                    if(!checkIsUserRoleValid(request.Role))
                    {
                        _logManager.InvalidUserRole(controllerName, actionName, request);
                        return BadRequest("Kullanıcı rolü admin, client ve editor dışında bir şey olamaz!");
                    }
                    var id = await _userService.CreateAndReturnIdAsync(request);
                    bool isCreated = await _userService.IsExistsAsync(id);

                    if (isCreated)
                    {
                        _logManager.SuccesfullCreate(controllerName, actionName, request);
                        return Ok();
                    }
                    _logManager.UnableCreate(controllerName, actionName, request);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                catch (InvalidOperationException e)
                {
                    _logManager.ExceptionOccured(controllerName, actionName, request, e.Message);
                    return Conflict(e.Message);
                }
                catch(Exception ex)
                {
                    _logManager.ExceptionOccured(controllerName, actionName, request, ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            _logManager.InvalidCreate(controllerName, actionName, request);
            return BadRequest(ModelState);
        }       

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateUserRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {                
                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    var isExist = await _userService.IsExistsAsync(request.Id);
                    if (isExist)
                    {
                        if (ModelState.IsValid)
                        {
                            await _userService.UpdateAsync(request);
                            _logManager.SuccesfullEdit(controllerName, actionName, request);
                            return Ok();
                        }
                        _logManager.InvalidEdit(controllerName, actionName, request);
                        return BadRequest(ModelState);
                    }
                    _logManager.NotExistInServer(controllerName, actionName, request);
                    return NotFound("Düzenlenmek istenen kullanıcı sunucuda bulunamadı!");
                }
                _logManager.BlankRequestId(controllerName, actionName, request);
                return BadRequest("İstek id boş!");
            }
            catch (Exception ex)
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
                bool isExist = await _userService.IsExistsAsync(id.ToString());
                if (!isExist)
                {
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                await _userService.DeleteAsync(id.ToString());

                isExist = await _userService.IsExistsAsync(id.ToString());
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
        public async Task<IActionResult> GetUser(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var userDisplay = await _userService.GetByIdAsync(id.ToString());

                if (userDisplay == null)
                {
                    _logManager.NotExistInServer(controllerName, actionName, id);
                    return NotFound();
                }
                _logManager.SuccesfullGet(controllerName, actionName, id);
                return Ok(userDisplay);
            }
            catch(Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, id, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetForEdit")]
        public async Task<IActionResult> GetUserForEdit(string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                bool isExist = await _userService.IsExistsAsync(id);
                if (isExist)
                {
                    var updateDisplay = await _userService.GetForUpdateAsync(id);
                    updateDisplay.Password = string.Empty;
                    _logManager.SuccesfullGet(controllerName, actionName, id);
                    return Ok(updateDisplay);
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
        [Route("Validate")]
        public async Task<IActionResult> ValidateUser(string email, string password)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try { 
                var user = await _userService.ValidateAsync(email, password);
                if (user is not null) 
                {
                    var loggedUser = await _userService.GetByIdAsync(user.Id);
                    _logManager.SuccesfullUserLogin(controllerName, actionName, user.Id);
                    return Ok(loggedUser);
                }
                _logManager.NotFoundUserLogin(controllerName, actionName, email, password);
                return NotFound();
            }
            catch (Exception ex) 
            {
                _logManager.ExceptionOccured(controllerName, actionName, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private bool checkIsUserRoleValid(string userRole)
        {
            if (userRole != UserRole.Admin && userRole != UserRole.Client && userRole != UserRole.Editor)
                    return false;            
            return true;
        }
    }
}
