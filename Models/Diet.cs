namespace GymTime.Models
{
    public class Diet
    {
        //Primary Key
        public int Id { get; set; }

        public string FoodName { get; set; }
        public int Proteins { get; set; }
        public int Fats { get; set; }
        public int Carbohydrates { get; set; }
        public int Calories  { get; set; }


    }
}
