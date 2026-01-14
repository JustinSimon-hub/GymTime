using System.ComponentModel.DataAnnotations;

namespace GymTime.Models.Data_Transfer_Object
{
    public class WorkoutDto
    {
        [Required(ErrorMessage = "Workout name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Workout name must be between 2 and 100 characters")]
        [Display(Name = "Workout Name")]
        public string WorkoutName { get; set; } = string.Empty;

        [Required]
        [Range(1, 100, ErrorMessage = "Reps must be between 1 and 100")]
        public int Reps { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = "Sets must be between 1 and 50")]
        public int Sets { get; set; }

        [Range(0, 1000, ErrorMessage = "Personal record must be between 0 and 1000 kg")]
        [Display(Name = "Personal Record (kg)")]
        public int PersonalRecord { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}