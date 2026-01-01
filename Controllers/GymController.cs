using GymTime.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Controllers
{
    public class GymController : Controller
    {
        private readonly IGymRepository repo;
        public GymController(IGymRepository repo)
        {
            this.repo = repo;
        }

        //Get /<controller>/ 
        public IActionResult Index()
        {
            var diets = repo.GetDiets();
            return View(diets);
        }
    }
}




