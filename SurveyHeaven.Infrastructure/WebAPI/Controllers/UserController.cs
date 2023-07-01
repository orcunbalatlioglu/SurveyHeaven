using Microsoft.AspNetCore.Mvc;
using SurveyHeaven.Application.DTOs.Requests;
using SurveyHeaven.Application.Services;
using SurveyHeaven.Domain.Entities;
using SurveyHeaven.Domain.Enums;

namespace WebAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {

            if (ModelState.IsValid)
            {
                try { 
                    if(!checkIsUserRoleValid(request.Role))
                    {
                        return BadRequest("Kullanıcı rolü admin, client ve editor dışında bir şey olamaz!");
                    }
                    var id = await _userService.CreateAndReturnIdAsync(request);
                    bool isCreated = await _userService.IsExistsAsync(id);

                    if (isCreated)
                    {
                        return Ok();
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.DeleteAsync(id.ToString());

            bool isExist = await _userService.IsExistsAsync(id.ToString());
            if (isExist)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok();
        }

        [HttpPut]
        [Route("Edit")]
        public async Task<IActionResult> Edit(UpdateUserRequest request, string id)
        {
            if (request != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Id))
                {
                    var isExist = await _userService.IsExistsAsync(request.Id);
                    if (isExist)
                    {
                        if (ModelState.IsValid)
                        {                            
                            await _userService.UpdateAsync(request);
                            return Ok();
                        }
                        return BadRequest(ModelState);
                    }
                    return NotFound("Düzenlenmek istenen kullanıcı sunucuda bulunamadı!");
                }
            }
            return BadRequest("İstek boş!");
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var userDisplay = await _userService.GetByIdAsync(id.ToString());

            if (userDisplay == null)
            {
                return NotFound();
            }

            return Ok(userDisplay);
        }

        [HttpGet]
        [Route("GetUserForEdit")]
        public async Task<IActionResult> GetUserForEdit(string id)
        {
            bool isExist = await _userService.IsExistsAsync(id);
            if (isExist)
            {
                var updateDisplay = await _userService.GetForUpdateAsync(id);
                updateDisplay.Password = string.Empty;
                return Ok(updateDisplay);
            }
            return NotFound();
        }

        [HttpGet]
        [Route("ValidateUser")]
        public async Task<IActionResult> ValidateUser(string email, string password)
        {
            var user = await _userService.ValidateAsync(email, password);
            if (user is not null) 
            {
                var loggedUser = await _userService.GetByIdAsync(user.Id);
                return Ok(loggedUser);
            }
            return NotFound();
        }

        private bool checkIsUserRoleValid(string userRole)
        {
            if (userRole != UserRole.Admin && userRole != UserRole.Client && userRole != UserRole.Editor)
                    return false;            
            return true;
        }
    }
}
