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

        public IEnumerable<Diet> GetDiets()
        {
            return _connection.Query<Diet>("SELECT * FROM Diets");
        }

        public IEnumerable<Workout> GetWorkouts()
        {
           return _connection.Query<Workout>("SELECT * FROM Workouts");
        }
    }
}
