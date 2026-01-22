using GymTime.Models;
using GymTime.Models.Data_Transfer_Object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymTime.ApiControllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Authorize] // ✅ Require authentication for ALL endpoints
    public class ApiDietController : ControllerBase
    {
        private readonly IGymRepository _repository;

        public ApiDietController(IGymRepository repository)
        {
            _repository = repository;
        }

        // ✅ Helper method to get authenticated user ID
        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        // ✅ Helper method to validate user access
        private bool ValidateUserAccess(int userId)
        {
            return GetAuthenticatedUserId() == userId;
        }

        //This will retreive all diets for a specific user
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Diet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<Diet>> GetDietsByUser(int userId)
        {
            // ✅ Ensure user can only access their own data
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            var diets = _repository.GetDietsByUser(userId);
            if (!diets.Any())
            {
                return NotFound(new { message = "No diets found for the specified user.", userId });
            }
            return Ok(diets);
        }

        //Will return specific diet entry
        [HttpGet("{id}/user/{userId}")]
        [ProducesResponseType(typeof(Diet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<Diet> GetDietByUser(int id, int userId)
        {
            // ✅ Ensure user can only access their own data
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            var diet = _repository.GetDietByUser(id, userId);
            if (diet == null)
            {
                return NotFound(new { mesasge = "Diet entry not found ", dietId = id, userId });
            }
            return Ok(diet);
        }

        //Returns the macro totals of a user
        [HttpGet("user/{userId}/macros")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<object> GetMacroTotal(int userId)
        {
            // ✅ Ensure user can only access their own data
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            var diets = _repository.GetDietsByUser(userId);

            var macroData = new
            {
                userId,
                totalProteins = diets.Sum(d => d.Proteins),
                totalCarbs = diets.Sum(d => d.Carbohydrates),
                totalFats = diets.Sum(d => d.Fats),
                totalCalories = diets.Sum(d => d.Calories),
                entryCount = diets.Count(),
                lastUpdated = DateTime.UtcNow
            };
            return Ok(macroData);
        }

        //Reponsible for creating a new Diet entry 
        [HttpPost("user/{userId}")]
        [ProducesResponseType(typeof(Diet), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<Diet> CreateDiet(int userId, [FromBody] DietDto dietDto)
        {
            // ✅ Ensure user can only create data for themselves
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid diet data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var diet = new Diet
            {
                UserId = userId,
                FoodName = dietDto.FoodName,
                Proteins = dietDto.Proteins,
                Fats = dietDto.Fats,
                Carbohydrates = dietDto.Carbohydrates,
                Calories = dietDto.Calories
            };
            _repository.InsertDiet(diet);
            return CreatedAtAction(
                nameof(GetDietByUser),
                new { id = diet.Id, userId = userId },
                diet);
        }

        ////Api gateway for updating diet entry 
        [HttpPut("{id}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult UpdateDiet(int id, int userId, [FromBody] DietDto dietDto)
        {
            // ✅ Ensure user can only update their own data
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid diet data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }
            var existingDiet = _repository.GetDietByUser(id, userId);
            if (existingDiet == null)
            {
                return NotFound(new { message = "Diet entry not found", dietId = id, userId });
            }
            existingDiet.FoodName = dietDto.FoodName;
            existingDiet.Proteins = dietDto.Proteins;
            existingDiet.Fats = dietDto.Fats;
            existingDiet.Carbohydrates = dietDto.Carbohydrates;
            existingDiet.Calories = dietDto.Calories;

            _repository.UpdateDiet(existingDiet);

            return NoContent();
        }

        //Delete diet gateway
        [HttpDelete("{id}/user/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult DeleteDiet(int id, int userId)
        {
            // ✅ Ensure user can only delete their own data
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            var existingDiet = _repository.GetDietByUser(id, userId);
            if (existingDiet == null)
            {
                return NotFound(new { message = "Diet entry not found", dietId = id, userId });
            }
            _repository.DeleteDietByUser(id, userId);
            return NoContent();
        }

        //Retrieve diet statistics
        [HttpGet("user/{userId}/stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<object> GetDietStats(int userId)
        {
            // ✅ Ensure user can only access their own stats
            if (!ValidateUserAccess(userId))
            {
                return Forbid();
            }

            var diets = _repository.GetDietsByUser(userId).ToList();
            if (!diets.Any())
            {
                return Ok(new { message = "No diet data available", userId });
            }
            var totalCalories = diets.Sum(d => d.Calories);
            var stats = new
            {
                userId,
                totalEntries = diets.Count,
                averageCaloriesPerEntry = diets.Average(d => d.Calories),
                highestCalorieFood = diets.OrderByDescending(d => d.Calories).FirstOrDefault()?.FoodName,
                highestProteinFood = diets.OrderByDescending(d => d.Proteins).FirstOrDefault()?.FoodName,
                totalDailyCalories = totalCalories,
                proteinPercentage = totalCalories > 0 ? Math.Round((double)(diets.Sum(d => d.Proteins) * 4) / totalCalories * 100, 1) : 0,
                carbsPercentage = totalCalories > 0 ? Math.Round((double)(diets.Sum(d => d.Carbohydrates) * 4) / totalCalories * 100, 1) : 0,
                fatsPercentage = totalCalories > 0 ? Math.Round((double)(diets.Sum(d => d.Fats) * 9) / totalCalories * 100, 1) : 0
            };

            return Ok(stats);
        }
    }
}