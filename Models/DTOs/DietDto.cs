using System.ComponentModel.DataAnnotations;

namespace GymTime.Models.Data_Transfer_Object
{

    public class DietDto
    {
        [Required(ErrorMessage = "Food name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Food name must be between 2 and 100 characters")]
        public string FoodName { get; set; } = string.Empty;

        [Required]
        [Range(0, 500, ErrorMessage = "Proteins must be between 0 and 500 grams")]
        [Display(Name = "Proteins (g)")]
        public int Proteins { get; set; }

        [Required]
        [Range(0, 300, ErrorMessage = "Fats must be between 0 and 300 grams")]
        [Display(Name = "Fats (g)")]
        public int Fats { get; set; }

        [Required]
        [Range(0, 500, ErrorMessage = "Carbohydrates must be between 0 and 500 grams")]
        [Display(Name = "Carbohydrates (g)")]
        public int Carbohydrates { get; set; }

        [Required]
        [Range(0, 5000, ErrorMessage = "Calories must be between 0 and 5000")]
        [Display(Name = "Calories (kcal)")]
        public int Calories { get; set; }
    }
}