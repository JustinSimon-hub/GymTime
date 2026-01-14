using GymTime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GymTime.Controllers.Filters;

namespace GymTime.Controllers
{
    [AuthorizeUser]  // 👈 Protects entire controller with ONE attribute
    public class GymController : Controller
    {
        private readonly IGymRepository repo;

        public GymController(IGymRepository repo)
        {
            this.repo = repo;
        }

        // Helper to get current user ID from session (now guaranteed to exist)
        private int CurrentUserId => HttpContext.Session.GetInt32("UserId")!.Value;

        // GET /Gym/
        public IActionResult Index()
        {
            var model = new GymViewModel
            {
                Diets = repo.GetDietsByUser(CurrentUserId),
                Workouts = repo.GetWorkoutsByUser(CurrentUserId)
            };
            return View(model);
        }

        // API endpoint for real-time macro data
        [HttpGet]
        public IActionResult GetMacroData()
        {
            var diets = repo.GetDietsByUser(CurrentUserId);
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
                var diet = repo.GetDietByUser(id, CurrentUserId);
            if (diet == null)
                return NotFound();

            return View(diet);
        }

        // GET /Gym/ViewWorkout/5
        public IActionResult ViewWorkout(int id)
        {
            var workout = repo.GetWorkoutByUser(id, CurrentUserId);
            if (workout == null)
                return NotFound();

            return View(workout);
        }

        public IActionResult UpdateDiet(int id)
        {
            var diet = repo.GetDietByUser(id, CurrentUserId);
            if (diet == null)
                return View("DietNotFound");

            return View(diet);
        }

        public IActionResult UpdateWorkout(int id)
        {
            var workout = repo.GetWorkoutByUser(id, CurrentUserId);
            if (workout == null)
                return View("WorkoutNotFound");

            return View(workout);
        }

        [HttpPost]
        public IActionResult UpdateWorkoutToDatabase(Workout workout)
        {
            workout.UserId = CurrentUserId;
            repo.UpdateWorkout(workout);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateDietToDatabase(Diet diet)
        {
            diet.UserId = CurrentUserId;
            repo.UpdateDiet(diet);
            return RedirectToAction("Index");
        }

        public IActionResult InsertWorkout()
        {
            return View();
        }

        public IActionResult InsertDiet()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertDietToDatabase(Diet diet)
        {
            diet.UserId = CurrentUserId;
            repo.InsertDiet(diet);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult InsertWorkoutToDatabase(Workout workout)
        {
            workout.UserId = CurrentUserId;
            repo.InsertWorkout(workout);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteDiet(int id)
        {
            repo.DeleteDietByUser(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteWorkout(int id)
        {
            repo.DeleteWorkoutByUser(id, CurrentUserId);
            return RedirectToAction("Index");
        }
    }
}
