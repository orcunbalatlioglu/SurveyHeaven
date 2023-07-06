using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Security.Claims;
using WebUI.Models;
using WebUI.Models.DTOs.Requests;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(CreateUserRequest request)
        {
            try { 
                _userService.CreateAsync(request);
                return RedirectToAction("SignUpSucces");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var loggedInfo = await _userService.LoginAsync(request);
                HttpContext.Session.SetString("Token", loggedInfo.Token);
                Claim[] claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name,loggedInfo.UserDisplay.Name),                    
                    new Claim(ClaimTypes.Surname,loggedInfo.UserDisplay.Surname),
                    new Claim(ClaimTypes.Email,loggedInfo.UserDisplay.Email),
                    new Claim(ClaimTypes.Role,loggedInfo.UserDisplay.Role)
                };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
                
                
                return RedirectToAction("Welcome","Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult SignUpSucces()
        {
            return View();
        }
    }
}
