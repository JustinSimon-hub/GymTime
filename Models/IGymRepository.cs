namespace GymTime.Models
{
    public interface IGymRepository
    {
       public IEnumerable<Workout> GetWorkouts();
        public IEnumerable<Diet> GetDiets();
        public Diet GetDiet(int id);
        public Workout GetWorkout(int id);

        public void UpdateDiet(Diet id);
        public void UpdateWorkout(Workout id);



    }
}
