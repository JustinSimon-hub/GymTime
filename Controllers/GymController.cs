using GymTime.Models;
using GymTime.Models.Data_Transfer_Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GymTime.Controllers.Filters;
using GymTime.Models.Data_Transfer_Object;

namespace GymTime.Controllers
{
    [AuthorizeUser]
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

            // Map entity to DTO
            var dietDto = new DietDto
            {
                Id = diet.Id,
                FoodName = diet.FoodName,
                Proteins = diet.Proteins,
                Fats = diet.Fats,
                Carbohydrates = diet.Carbohydrates,
                Calories = diet.Calories
            };

            return View(dietDto);
        }

        public IActionResult UpdateWorkout(int id)
        {
            var workout = repo.GetWorkoutByUser(id, CurrentUserId);
            if (workout == null)
                return View("WorkoutNotFound");

            // Map entity to DTO
            var workoutDto = new WorkoutDto
            {
                Id = workout.Id,
                WorkoutName = workout.WorkoutName,
                Reps = workout.Reps,
                Sets = workout.Sets,
                PersonalRecord = workout.PersonalRecord,
                Description = workout.Description
            };

            return View(workoutDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateWorkoutToDatabase(int id, WorkoutDto workoutDto)
        {
            if (!ModelState.IsValid)
            {
                //Using workoutDto as the model in user formm for this view
                return View("UpdateWorkout", workoutDto);
            }

            var workout = repo.GetWorkoutByUser(id, CurrentUserId);
            if (workout == null)
                return NotFound();

            // Map DTO to entity
            workout.WorkoutName = workoutDto.WorkoutName;
            workout.Reps = workoutDto.Reps;
            workout.Sets = workoutDto.Sets;
            workout.PersonalRecord = workoutDto.PersonalRecord;
            workout.Description = workoutDto.Description ?? string.Empty;

            repo.UpdateWorkout(workout);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDietToDatabase(int id, DietDto dietDto)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateDiet", dietDto);
            }

            var diet = repo.GetDietByUser(id, CurrentUserId);
            if (diet == null)
                return NotFound();

            // Map DTO to entity
            diet.FoodName = dietDto.FoodName;
            diet.Proteins = dietDto.Proteins;
            diet.Fats = dietDto.Fats;
            diet.Carbohydrates = dietDto.Carbohydrates;
            diet.Calories = dietDto.Calories;

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
        [ValidateAntiForgeryToken]
        public IActionResult InsertDietToDatabase(DietDto dietDto)
        {
            if (!ModelState.IsValid)
            {
                return View("InsertDiet", dietDto);
            }

            var diet = new Diet
            {
                UserId = CurrentUserId,
                FoodName = dietDto.FoodName,
                Proteins = dietDto.Proteins,
                Fats = dietDto.Fats,
                Carbohydrates = dietDto.Carbohydrates,
                Calories = dietDto.Calories
            };

            repo.InsertDiet(diet);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertWorkoutToDatabase(WorkoutDto workoutDto)
        {
            if (!ModelState.IsValid)
            {
                return View("InsertWorkout", workoutDto);
            }

            var workout = new Workout
            {
                UserId = CurrentUserId,
                WorkoutName = workoutDto.WorkoutName,
                Reps = workoutDto.Reps,
                Sets = workoutDto.Sets,
                PersonalRecord = workoutDto.PersonalRecord,
                Description = workoutDto.Description ?? string.Empty
            };

            repo.InsertWorkout(workout);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteDiet(int id)
        {
            repo.DeleteDietByUser(id, CurrentUserId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteWorkout(int id)
        {
            repo.DeleteWorkoutByUser(id, CurrentUserId);
            return RedirectToAction("Index");
        }
    }
}
