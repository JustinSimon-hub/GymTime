using GymTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GymTime.Controllers
{
    public class GymController : Controller
    {
        private readonly IGymRepository repo;

        public GymController(IGymRepository repo)
        {
            this.repo = repo;
        }

        // Helper to get current user ID from session
        private int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        // GET /Gym/
        public IActionResult Index()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            var model = new GymViewModel
            {
                Diets = repo.GetDietsByUser(CurrentUserId.Value),
                Workouts = repo.GetWorkoutsByUser(CurrentUserId.Value)
            };
            return View(model);
        }

        // API endpoint for real-time macro data
        [HttpGet]
        public IActionResult GetMacroData()
        {
            if (CurrentUserId == null)
                return Unauthorized();

            var diets = repo.GetDietsByUser(CurrentUserId.Value);
            var macroData = new
            {
                totalProteins = diets.Sum(d => d.Proteins),
                totalCarbs = diets.Sum(d => d.Carbohydrates),
                totalFats = diets.Sum(d => d.Fats),
                totalCalories = diets.Sum(d => d.Calories)
            };
            return Json(macroData);
        }

        // GET /Gym/ViewDiet/5
        public IActionResult ViewDiet(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var diet = repo.GetDietByUser(id, userId.Value);
            if (diet == null)
                return Unauthorized();

            return View(diet);
        }


        // GET /Gym/ViewWorkout/5
        public IActionResult ViewWorkout(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var workout = repo.GetWorkoutByUser(id, userId.Value);
            if (workout == null)
                return Unauthorized();

            return View(workout);
        }


        public IActionResult UpdateDiet(int id)
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            var diet = repo.GetDietByUser(id, CurrentUserId.Value);
            if (diet == null)
                return View("DietNotFound");

            return View(diet);
        }

        public IActionResult UpdateWorkout(int id)
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            var workout = repo.GetWorkoutByUser(id, CurrentUserId.Value);
            if (workout == null)
                return View("WorkoutNotFound");

            return View(workout);
        }


        [HttpPost]
        public IActionResult UpdateWorkoutToDatabase(Workout workout)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            workout.UserId = userId.Value;

            repo.UpdateWorkout(workout);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult UpdateDietToDatabase(Diet diet)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            diet.UserId = userId.Value;

            repo.UpdateDiet(diet);
            return RedirectToAction("Index");
        }


        public IActionResult InsertWorkout()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public IActionResult InsertDiet()
        {
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public IActionResult InsertDietToDatabase(Diet diet)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
                return RedirectToAction("Login", "Account");

            diet.UserId = currentUserId.Value;

            repo.InsertDiet(diet);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult InsertWorkoutToDatabase(Workout workout)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (currentUserId == null)
                return RedirectToAction("Login", "Account");

            workout.UserId = currentUserId.Value;

            repo.InsertWorkout(workout);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteDiet(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            repo.DeleteDietByUser(id, userId.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteWorkout(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            repo.DeleteWorkoutByUser(id, userId.Value);
            return RedirectToAction("Index");
        }

    }
}
