using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.WebAPI.Logger;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

//TODO: Jwt parametrelerini incele.
//TODO: Hangfire'ı bütün projede kullanmaya çalış.
//TODO: Giriş sırasında email ve şifrenin gözükmemesini sağla.

namespace SurveyHeaven.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUserLogManager _logManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string JwtKey;

        public UserController(IUserService userService,
                              IUserLogManager logManager,
                              IConfiguration configuration,
                              IHttpContextAccessor contextAccessor)
        {
            _userService = userService;
            _logManager = logManager;
            _contextAccessor = contextAccessor;
            JwtKey = configuration.GetSection("JwtKey").ToString();
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (ModelState.IsValid)
            {
                try {
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("CreateEditor")]
        public async Task<IActionResult> CreateEditor(CreateUserRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (ModelState.IsValid)
            {
                try
                {
                    var id = await _userService.CreateEditorAndReturnIdAsync(request);
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
                catch (Exception ex)
                {
                    _logManager.ExceptionOccured(controllerName, actionName, request, ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            _logManager.InvalidCreate(controllerName, actionName, request);
            return BadRequest(ModelState);
        }

        [Authorize(Roles ="admin")]
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

        [Authorize(Roles = "client")]
        [HttpPut]
        [Route("ProfileEdit")]
        public async Task<IActionResult> ProfileEdit(UpdateUserRequest request)
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
                            var signedInUserId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                            if(request.Id == signedInUserId) 
                            { 
                                await _userService.UpdateAsync(request);
                                _logManager.SuccesfullEdit(controllerName, actionName, request);
                                return Ok();
                            }

                            _logManager.UnauthorizedAccessTry(controllerName, actionName, signedInUserId, request.Id);
                            return Forbid();
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin,editor")]
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

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles ="client")]
        [HttpGet]
        [Route("GetProfileForEdit")]
        public async Task<IActionResult> GetProfileForEdit()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            var signedInUserId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (signedInUserId == null)
            {
                _logManager.NotFoundSignedInUserInServer(controllerName, actionName,signedInUserId);
                return NotFound("Giriş yapmış olan kullanıcı id sunucuda bulunamamıştır.");
            }

            try
            {
                
                bool isExist = await _userService.IsExistsAsync(signedInUserId);
                if (isExist)
                {                    
                    var updateDisplay = await _userService.GetForUpdateAsync(signedInUserId);
                    updateDisplay.Password = string.Empty;
                    _logManager.SuccesfullGet(controllerName, actionName, signedInUserId);
                    return Ok(updateDisplay);                    
                }
                _logManager.NotExistInServer(controllerName, actionName, signedInUserId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logManager.ExceptionOccured(controllerName, actionName, signedInUserId, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize(Roles = "admin,editor")]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                var userDisplays = await _userService.GetAllAsync();

                if (userDisplays.Count() > 0)
                {
                    _logManager.SuccesfullGetAll(controllerName, actionName);
                    return Ok(userDisplays);                    
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

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try { 
                var validatedUserInfo = await _userService.ValidateAsync(request.Email, request.Password, JwtKey);
                if (validatedUserInfo.ContainsKey("Token")) 
                {
                    if (validatedUserInfo.ContainsKey("UserId"))
                    { 
                        string token = validatedUserInfo["Token"];
                        string userId = validatedUserInfo["UserId"];
                        var userDisplay = await _userService.GetByIdAsync(userId);
                        _logManager.SuccesfullUserLogin(controllerName, actionName, userId);
                        return Ok(new { token, userDisplay });
                    }
                }
                _logManager.NotFoundUserLogin(controllerName, actionName, @request);
                return NotFound();
            }
            catch (Exception ex) 
            {
                _logManager.ExceptionOccured(controllerName, actionName, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
