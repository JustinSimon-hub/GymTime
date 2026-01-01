namespace GymTime.Models
{
    public interface IGymRepository
    {
       public IEnumerable<Workout> GetWorkouts();
        public IEnumerable<Diet> GetDiets();
        public Diet GetDiet(int id);
        public Workout GetWorkout(int id);

    }
}
