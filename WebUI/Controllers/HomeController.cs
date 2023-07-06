using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Error(ErrorViewModel error)
        {
            return View(error);
        }
    }

}
