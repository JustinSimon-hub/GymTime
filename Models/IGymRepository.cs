namespace GymTime.Models
{
    public interface IGymRepository
    {
       public IEnumerable<Workout> GetWorkouts();
        public IEnumerable<Diet> GetDiets();
        

    }
}
