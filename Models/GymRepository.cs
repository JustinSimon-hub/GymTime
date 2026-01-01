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
    }
}
