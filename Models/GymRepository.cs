using System;
using System.Data;
using Dapper;

namespace GymTime.Models
{
    public class GymRepository : IGymRepository
    {
        private readonly IDbConnection _connection;

        public GymRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        // ----------------------
        // GET SINGLE / MULTIPLE
        // ----------------------

        public Diet? GetDiet(int id)
        {
            return _connection.QuerySingleOrDefault<Diet>(
                "SELECT * FROM Diets WHERE Id = @id", new { id }
            );
        }

        public Diet? GetDietByUser(int id, int userId)
        {
            return _connection.QuerySingleOrDefault<Diet>(
                "SELECT * FROM Diets WHERE Id = @id AND UserId = @userId",
                new { id, userId }
            );
        }

        public IEnumerable<Diet> GetDiets()
        {
            // Non-user-specific (optional)
            return _connection.Query<Diet>("SELECT * FROM Diets");
        }

        public IEnumerable<Diet> GetDietsByUser(int userId)
        {
            return _connection.Query<Diet>(
                "SELECT * FROM Diets WHERE UserId = @userId",
                new { userId }
            );
        }

        public Workout? GetWorkout(int id)
        {
            return _connection.QuerySingleOrDefault<Workout>(
                "SELECT * FROM Workouts WHERE Id = @id", new { id }
            );
        }

        public Workout? GetWorkoutByUser(int id, int userId)
        {
            return _connection.QuerySingleOrDefault<Workout>(
                "SELECT * FROM Workouts WHERE Id = @id AND UserId = @userId",
                new { id, userId }
            );
        }

        public IEnumerable<Workout> GetWorkouts()
        {
            // Non-user-specific (optional)
            return _connection.Query<Workout>("SELECT * FROM Workouts");
        }

        public IEnumerable<Workout> GetWorkoutsByUser(int userId)
        {
            return _connection.Query<Workout>(
                "SELECT * FROM Workouts WHERE UserId = @userId",
                new { userId }
            );
        }

        // ----------------------
        // INSERT
        // ----------------------

        public void InsertDiet(Diet diet)
        {
            _connection.Execute(
                @"INSERT INTO Diets (FoodName, Proteins, Carbohydrates, Calories, Fats, UserId)
                  VALUES (@FoodName, @Proteins, @Carbohydrates, @Calories, @Fats, @UserId)",
                diet
            );
        }

        public void InsertWorkout(Workout workout)
        {
            _connection.Execute(
                @"INSERT INTO Workouts (WorkoutName, Reps, Sets, PersonalRecord, Description, UserId)
                  VALUES (@WorkoutName, @Reps, @Sets, @PersonalRecord, @Description, @UserId)",
                workout
            );
        }

        // ----------------------
        // UPDATE
        // ----------------------

        public void UpdateDiet(Diet diet)
        {
            _connection.Execute(
                @"UPDATE Diets
                  SET FoodName = @FoodName,
                      Proteins = @Proteins,
                      Carbohydrates = @Carbohydrates,
                      Calories = @Calories,
                      Fats = @Fats
                  WHERE Id = @Id AND UserId = @UserId",
                diet
            );
        }

        public void UpdateWorkout(Workout workout)
        {
            _connection.Execute(
                @"UPDATE Workouts
                  SET WorkoutName = @WorkoutName,
                      Reps = @Reps,
                      Sets = @Sets,
                      PersonalRecord = @PersonalRecord,
                      Description = @Description
                  WHERE Id = @Id AND UserId = @UserId",
                workout
            );
        }

        // ----------------------
        // DELETE
        // ----------------------

        public void DeleteDiet(int id)
        {
            // Non-user-specific (optional)
            _connection.Execute("DELETE FROM Diets WHERE Id = @id", new { id });
        }

        public void DeleteDietByUser(int id, int userId)
        {
            _connection.Execute(
                "DELETE FROM Diets WHERE Id = @id AND UserId = @userId",
                new { id, userId }
            );
        }

        public void DeleteWorkout(int id)
        {
            // Non-user-specific (optional)
            _connection.Execute("DELETE FROM Workouts WHERE Id = @id", new { id });
        }

        public void DeleteWorkoutByUser(int id, int userId)
        {
            _connection.Execute(
                "DELETE FROM Workouts WHERE Id = @id AND UserId = @userId",
                new { id, userId }
            );
        }
    }
}
