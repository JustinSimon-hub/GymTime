namespace GymTime.Models
{
    public class Workout
    {


        public int Id { get; set; }
        public int UserId { get; set; }

        public string WorkoutName { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public int PersonalRecord { get; set; }
        public string Description { get; set; }


    }
}
