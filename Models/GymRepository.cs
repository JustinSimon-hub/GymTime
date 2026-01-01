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
            throw new NotImplementedException();
        }

        public IEnumerable<Workout> GetWorkouts()
        {
           return _connection.Query<Workout>("SELECT * FROM Workouts");
        }

        public void UpdateDiet(Diet diet)
        {
             _connection.Execute("UPDATE Diets SET FoodName = @FoodName, Proteins = @Proteins, Carbohydrates = @Carbohydrates, Calories = @Calories WHERE Id = @Id",
                new { Id = diet.Id, FoodName = diet.FoodName, Proteins = diet.Proteins, Carbohydrates = diet.Carbohyradates, Calories = diet.Calories });
        }

        public void UpdateWorkout(Workout id)
        {
            throw new NotImplementedException();
        }
    }
}
