using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace GymTime.Models.ApiAuth
{
    public class JwtAuthorization
    {
        private readonly IConfiguration _configuration;
        public JwtAuthorization(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public string GenerateToken(int userId, string username)
        //{
        //    var securityKey = new SymmetricSecurityKey()
        //}



    }
}
