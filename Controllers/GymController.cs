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
            var model = new GymViewModel
            {
                Diets = repo.GetDiets(),
                Workouts = repo.GetWorkouts()
            };
            return View(model);
        }

        // API endpoint for real-time macro data
        [HttpGet]
        public IActionResult GetMacroData()
        {
            var diets = repo.GetDiets();
            var macroData = new
            {
                totalProteins = diets.Sum(d => d.Proteins),
                totalCarbs = diets.Sum(d => d.Carbohydrates),
                totalFats = diets.Sum(d => d.Fats),
                totalCalories = diets.Sum(d => d.Calories)
            };
            return Json(macroData);
        }

        //Get /<controller>/ViewDiet/
        public IActionResult ViewDiet(int id)
        {
            var diet = repo.GetDiet(id);
            return View(diet);

        }
        public IActionResult ViewWorkout(int id)
        {
            var workout = repo.GetWorkout(id);
            return View(workout);
        }



        public IActionResult UpdateDiet(int id)
        {
            Diet diet = repo.GetDiet(id);
            if (diet == null)
            {
                return View("DietNotFound");
            }
            return View(diet);  
        }



        public IActionResult UpdateWorkout(int id)
        {
            var workout = repo.GetWorkout(id);
            if (workout == null)
            {
                return View("WorkoutNotFound");
            }

            return View(workout);
        }



        [HttpPost]
        public IActionResult UpdateWorkoutToDatabase(Workout workout)
        {
            repo.UpdateWorkout(workout);
            return RedirectToAction("ViewWorkout", new {id = workout.Id});
        }

        [HttpPost]
        public IActionResult UpdateDietToDatabase(Diet diet)
        {
            repo.UpdateDiet(diet);
            return RedirectToAction("ViewDiet", new {id = diet.Id});
        }


        public IActionResult InsertWorkout()
        {
            return View();
        }

        public IActionResult InsertDiet()
        {
            
            return View();
        }

        public IActionResult InsertDietToDatabase(Diet DietToInsert)
        {
            repo.InsertDiet(DietToInsert);
            return RedirectToAction("Index");
        }
        public IActionResult InsertWorkoutToDatabase(Workout WorkoutToInsert)
        {
            repo.InsertWorkout(WorkoutToInsert);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteDiet(int id)
        {

            repo.DeleteDiet(id);
            return RedirectToAction("Index");
        }


        public IActionResult DeleteWorkout(int id)
        {
            repo.DeleteWorkout(id);
            return RedirectToAction("Index");
        }   
        

    }
}




