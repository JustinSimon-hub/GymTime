using Dapper;
using System;
using System.Data;

namespace GymTime.Models
{
    public class GymRepository : IGymRepository
    {
        private readonly IDbConnection _connection;

        public GymRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        

        public Diet GetDiet(int id) 
        {

            return _connection.QuerySingle<Diet>("SELECT * FROM Diets WHERE Id = @id",
               new { id = id });

        }

        public IEnumerable<Diet> GetDiets()
        {
            return _connection.Query<Diet>("SELECT * FROM Diets");
        }

        public Workout GetWorkout(int id)
        {
            return _connection.QuerySingle<Workout>("SELECT * FROM Workouts WHERE ID = @id",
            new { id = id });
        }

        public IEnumerable<Workout> GetWorkouts()
        {
           return _connection.Query<Workout>("SELECT * FROM Workouts");
        }

        public void InsertDiet(Diet diet)
        {
           
              _connection.Execute("INSERT INTO Diets (FoodName, Proteins, Carbohydrates, Calories, Fats) VALUES (@FoodName, @Proteins, @Carbohydrates, @Calories, @Fats)",
                new { FoodName = diet.FoodName, Proteins = diet.Proteins, Carbohydrates = diet.Carbohydrates, Calories = diet.Calories, Fats = diet.Fats });
        }

        public void InsertWorkout(Workout workout)
        {
            _connection.Execute("INSERT INTO Workouts (WorkoutName, Reps, Sets, PersonalRecord, Description) VALUES (@WorkoutName, @Reps, @Sets, @PersonalRecord, @Description)",
                new { WorkoutName = workout.WorkoutName, Reps = workout.Reps, Sets = workout.Sets, PersonalRecord = workout.PersonalRecord, Description = workout.Description }); 
        }

        public void UpdateDiet(Diet diet)
        {
             _connection.Execute("UPDATE Diets SET FoodName = @FoodName, Proteins = @Proteins, Carbohydrates = @Carbohydrates, Calories = @Calories WHERE Id = @Id",
                new { Id = diet.Id, FoodName = diet.FoodName, Proteins = diet.Proteins, Carbohydrates = diet.Carbohydrates, Calories = diet.Calories });
        }

        public void UpdateWorkout(Workout workout)
        {
            _connection.Execute("UPDATE Workouts SET WorkoutName = @WorkoutName, Reps = @Reps, Sets = @Sets, PersonalRecord = @PersonalRecord WHERE Id = @Id",
                 new { WorkoutName = workout.WorkoutName, Reps = workout.Reps, Sets = workout.Sets, PersonalRecord = workout.PersonalRecord, Description = workout.Description });
        }


   //Delete actions

        public void DeleteDiet(int id)
        {
            _connection.Execute("DELETE FROM Diets WHERE Id = @id",
                new { id = id });

        }

        public void DeleteWorkout(int id)
        {
            _connection.Execute("DELETE FROM Workouts WHERE Id = @id",
              new { id = id });
        }
    }
}
