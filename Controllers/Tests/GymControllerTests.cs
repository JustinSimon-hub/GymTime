using Microsoft.AspNetCore.Mvc;

namespace GymTime.Controllers.Tests
{
    public class GymControllerTests : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
