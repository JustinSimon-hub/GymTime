namespace GymTime.Models
{
    /*Created to accomondate the additional 
     Models to be used inside the
    Index page*/
    public class GymViewModel
    {
        public IEnumerable<Diet> Diets { get; set; }
        public IEnumerable<Workout> Workouts { get; set; }
    }
}
