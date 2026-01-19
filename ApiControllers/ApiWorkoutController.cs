using GymTime.Models;
using GymTime.Models.Data_Transfer_Object;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.ApiControllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ApiWorkoutController : ControllerBase
    {
        private readonly IGymRepository _repository;

        public ApiWorkoutController(IGymRepository repository)
        {
            _repository = repository;
        }

        // This will return all workout data for a specific user
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Workout>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Workout>> GetWorkoutsByUser(int userId) 
        {
            var workouts = _repository.GetWorkoutsByUser(userId); 
            if (!workouts.Any())
            {
                return NotFound(new { message = "No workouts found for the specified user.", userId });
            }
            return Ok(workouts);
        }

        // Only returns workout data for one specific user
        [HttpGet("{id}/user/{userId}")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Workout> GetWorkoutByUser(int id, int userId) 
        {
            var workout = _repository.GetWorkoutByUser(id, userId);
            if (workout == null)
            {
                return NotFound(new { message = "Workout data not found.", workoutId = id, userId });
            }
            return Ok(workout);
        }

        // Creates a new workout entry
        [HttpPost("user/{userId}")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Workout> CreateWorkout(int userId, [FromBody] WorkoutDto workoutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid workout data",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var workout = new Workout
            {
                UserId = userId,
                WorkoutName = workoutDto.WorkoutName,
                Reps = workoutDto.Reps,
                Sets = workoutDto.Sets,
                PersonalRecord = workoutDto.PersonalRecord,
                Description = workoutDto.Description ?? string.Empty
            };

            _repository.InsertWorkout(workout);

            return CreatedAtAction(
                nameof(GetWorkoutByUser),
                new { id = workout.Id, userId = userId },
                workout);
        }

        // Gateway for updating workout
        [HttpPut("{id}/user/{userId}")] // ✅ Fixed: added {id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateWorkout(int id, int userId, [FromBody] WorkoutDto workoutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Invalid Workout data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var existingWorkout = _repository.GetWorkoutByUser(id, userId);
            if (existingWorkout == null)
            {
                return NotFound(new { message = "Workout entry not found", workoutId = id, userId });
            }

            existingWorkout.WorkoutName = workoutDto.WorkoutName;
            existingWorkout.Reps = workoutDto.Reps;
            existingWorkout.Sets = workoutDto.Sets;
            existingWorkout.PersonalRecord = workoutDto.PersonalRecord;
            existingWorkout.Description = workoutDto.Description ?? string.Empty; 

            _repository.UpdateWorkout(existingWorkout);
            return NoContent();
        }

        // Delete workout gateway
        [HttpDelete("{id}/user/{userId}")] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteWorkout(int id, int userId) 
        {
            var existingWorkout = _repository.GetWorkoutByUser(id, userId); 
            if (existingWorkout == null)
            {
                return NotFound(new { message = "Workout entry not found.", workoutId = id, userId });
            }

            _repository.DeleteWorkoutByUser(id, userId);
            return NoContent();
        }

        // Retrieve workout statistics
        [HttpGet("user/{userId}/stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetWorkoutStats(int userId)
        {
            var workouts = _repository.GetWorkoutsByUser(userId).ToList();
            if (!workouts.Any())
            {
                return Ok(new { message = "No workout data available", userId });
            }

            var stats = new
            {
                userId,
                totalWorkouts = workouts.Count,
                totalVolume = workouts.Sum(x => x.Reps * x.Sets * x.PersonalRecord),
                averageRepsPerWorkout = workouts.Average(x => x.Reps),
                averageSetsPerWorkout = workouts.Average(x => x.Sets),
                highestPersonalRecord = workouts.Max(x => x.PersonalRecord),
                strongestExercise = workouts.OrderByDescending(x => x.PersonalRecord).FirstOrDefault()?.WorkoutName,
                weakestExercise = workouts.OrderBy(x => x.PersonalRecord).FirstOrDefault()?.WorkoutName,
                uniqueExercises = workouts.Select(x => x.WorkoutName).Distinct().Count()
            };

            return Ok(stats);
        }

        // Retrieves personal records by exercise for user
        [HttpGet("user/{userId}/records")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetPersonalRecords(int userId)
        {
            var workouts = _repository.GetWorkoutsByUser( userId).ToList();

            if (!workouts.Any())
            {
                return Ok(new { message = "No workout records available", userId });
            }

            var records = workouts
                .GroupBy(w => w.WorkoutName)
                .Select(g => new
                {
                    exerciseName = g.Key,
                    maxWeight = g.Max(w => w.PersonalRecord),
                    maxReps = g.Max(w => w.Reps),
                    totalSessions = g.Count(),
                    averageReps = g.Average(w => w.Reps),
                    averageSets = g.Average(w => w.Sets)
                })
                .OrderByDescending(r => r.maxWeight);

            return Ok(records);
        }
    }
}