using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Enums;

//TODO: Kullanıcı girişinde JWT işlemlerini yap.
//TODO: İşlemelere role based authorize getir.
//TODO: Hangfire'ı bütün projede kullanmaya çalış.

namespace WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, 
                              ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
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
                        _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğindeki kullanıcı rolü geçersiz olduğundan yeni bir varlık oluşturulmasına izin verilmemiştir.");
                        return BadRequest("Kullanıcı rolü admin, client ve editor dışında bir şey olamaz!");
                    }
                    var id = await _userService.CreateAndReturnIdAsync(request);
                    bool isCreated = await _userService.IsExistsAsync(id);

                    if (isCreated)
                    {
                        _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucuda yeni bir varlık oluşturulmuştur.");
                        return Ok();
                    }
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında belirlemeyen bir sebepten dolayı yeni bir varlık oluşturulamamıştır.");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                catch (InvalidOperationException e)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {e.Message}");
                    return Conflict(e.Message);
                }
                catch(Exception ex)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için yeni bir varlık oluşturulamamıştır.");
            return BadRequest(ModelState);
        }       

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateUserRequest request, string id)
        {
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {                
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var isExist = await _userService.IsExistsAsync(request.Id);
                    if (isExist)
                    {
                        if (ModelState.IsValid)
                        {
                            await _userService.UpdateAsync(request);
                            _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında başarıyla sunucudaki varlık düzenlenmiştir.");
                            return Ok();
                        }
                        _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteğinde hatalar olduğu için varlık düzenlememiştir.");
                        return BadRequest(ModelState);
                    }
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde {@request} isteği karşılığında sunucuda belirtilen id ile eşleşen bir varlık bulunamamıştır.");
                    return NotFound("Düzenlenmek istenen kullanıcı sunucuda bulunamadı!");
                }
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde id boş olduğu için işlem gerçekleştirilememiştir.");
                return BadRequest("İstek boş!");
            }
            catch (Exception ex)
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
                bool isExist = await _userService.IsExistsAsync(id.ToString());
                if (!isExist)
                {
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                await _userService.DeleteAsync(id.ToString());

                isExist = await _userService.IsExistsAsync(id.ToString());
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
                    _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                    return NotFound();
                }
                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
                return Ok(userDisplay);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde Id:{id} değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
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
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki id değeri ({id}) ile sunucuda eşleşen varlık başarılı bir şekilde kullanıcıya iletilmiştir.");
                    return Ok(updateDisplay);
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
                    _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki email:({email}) ve password:({password}) ile sunucuda eşleşen varlık bulunmuştur. Kullanıcının girişi başarılı bir şekilde tamamlanmıştır.");
                    return Ok(loggedUser);
                }
                _logger.LogInformation($"{controllerName} kontrolcüsünde {actionName} işleminde istek içerisindeki email:({email}) ve password:({password}) ile sunucuda eşleşen bir varlık bulunamamıştır.");
                return NotFound();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"{controllerName} kontrolcüsünde {actionName} işleminde email:({email}) ve password:({password}) değerine sahip varlığın istenilen işlemi gerçekleştirilmesi sırasında bir hata ile karşılaşılmıştır. Hata mesajı: {ex.Message}");
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
