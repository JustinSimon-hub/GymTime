using GymTime.Models;
using GymTime.Models.Data_Transfer_Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
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
        //This will return all workout data for a specific user
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<Workout>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Workout>> GetWorkoutsByUser(int userId)
        {
            var workouts = _repository.GetWorkoutsByUser(userId);
            if(!workouts.Any())
            {
                return NotFound(new { message = "No workouts found for the specified user.", userId });
            }
            return Ok(workouts);
        }

        //Only returns workout data for one specific user
        [HttpGet("{id}/user/{userId}")]
        [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetWorkoutByUser(int id, int userId)
        {   var workout = _repository.GetWorkoutByUser(id, userId);
            if (workout == null)
            {
                return NotFound(new { message = "Workout data not found." });
            }
            return Ok(workout);
        }




        //Creates a new workout entry
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





    }































    }


    
}
