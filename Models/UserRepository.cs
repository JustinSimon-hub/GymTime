using Dapper;
using Microsoft.Extensions.Validation;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace GymTime.Models
{
    public class UserRepository
    {
        private readonly IDbConnection _connection;
        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

       public void Register( string email, string password)
        { 
           var hash = BCrypt.Net.BCrypt.HashPassword(password);
            _connection.Execute(@"INSERT INTO Users (Email, PasswordHash)
                VALUES (@Email, @PasswordHash)",
                new { Email = email, PasswordHash = hash });
        }


        public User? Login(string email, string password)
        {
            var user = _connection.QuerySingleOrDefault<User>(
                "SELECT * FROM Users WHERE Email = @Email",
                new { Email = email });
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;

            return null;

        }


        //Using to Account Controller to check existence
        public User? GetByEmail(string email)
        {
            return _connection.QuerySingleOrDefault<User>(
                "SELECT * FROM Users WHERE Email = @Email",
                new { Email = email }
            );
        }

    }
}
