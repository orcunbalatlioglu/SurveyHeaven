using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebUI.Models;
using WebUI.Models.DTOs.Requests;
using WebUI.Services;
using Newtonsoft.Json.Linq;

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
                if(ModelState.IsValid) 
                { 
                    _userService.CreateAsync(request);
                    return RedirectToAction("SignUpSucces");
                }
                return View();

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
                if (ModelState.IsValid) {
                    var jwtToken = await _userService.LoginAsync(request);

                    var jwtHandler = new JwtSecurityTokenHandler();
                    var token = jwtHandler.ReadJwtToken(jwtToken.Token);

                    var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == "nameid");
                    var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "email");
                    var nameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
                    var surnameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "family_name");
                    var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "role");

                    Claim[] claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, idClaim.Value),
                        new Claim(ClaimTypes.Name,nameClaim.Value),                    
                        new Claim(ClaimTypes.Surname,surnameClaim.Value),
                        new Claim(ClaimTypes.Email,emailClaim.Value),
                        new Claim(ClaimTypes.Role,roleClaim.Value),
                        new Claim(ClaimTypes.Upn, jwtToken.Token)
                    };
                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(principal);
                
                
                    return RedirectToAction("Welcome","Home"); 
                }
                return View();
            }
            catch (HttpRequestException ex)
            {
                if(ex.StatusCode == HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError("login", "Kullanıcı adı veya şifre hatalı.");
                    return View();
                }
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        public async Task<IActionResult> Profile()
        {
            try 
            { 
                var response = await _userService.GetProfileAsync();
                return View(response);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = ex.Message });
            }
        }

        public IActionResult SignUpSucces()
        {
            return View();
        }
    }
}
