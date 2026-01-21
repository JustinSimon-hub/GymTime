using Microsoft.AspNetCore.Mvc;
using GymTime.Models;
using GymTime.Models.ApiAuth;
using Microsoft.AspNetCore.Identity.Data;
using System.Runtime.CompilerServices;

namespace GymTime.ApiControllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ApiAuthController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly JwtServices _jwtService;
        //Dep Inj.
        public ApiAuthController(UserRepository userRepository, JwtServices jwtServices)
        {
            _userRepository = userRepository;
            _jwtService = jwtServices;
        }
        //Login endpoint, gives user Jwt token 
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if(string.IsNullOrEmpty(request.Email))

                //return to finish 

            return View();

        }
            






    }

}
