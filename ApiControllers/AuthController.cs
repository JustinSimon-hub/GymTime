using Microsoft.AspNetCore.Mvc;

namespace GymTime.ApiControllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
