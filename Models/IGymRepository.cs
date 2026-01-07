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

        public void InsertDiet(Diet diet);  
        public void InsertWorkout(Workout workout);

        public void DeleteDiet(int id); 
        public void DeleteWorkout(int id);



    }
}
