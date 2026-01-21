using GymTime.Models;
using GymTime.Models.ApiAuth;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.ApiControllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ApiAuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtServices _jwtService;

        public ApiAuthController(UserRepository userRepository, JwtServices jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        // Login endpoint - generates JWT token
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            var user = _userRepository.Login(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtService.GenerateToken(user.ID, user.Email);

            return Ok(new
            {
                token,
                userId = user.ID,
                email = user.Email,
                expiresIn = 3600 // seconds
            });
        }

        // Registration endpoint
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            // Check if user already exists
            var existingUser = _userRepository.GetByEmail(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already exists" });
            }

            _userRepository.Register(request.Email, request.Password);

            // Retrieve the newly created user
            var user = _userRepository.GetByEmail(request.Email);

            var token = _jwtService.GenerateToken(user.ID, user.Email);

            return CreatedAtAction(nameof(Login), new
            {
                token,
                userId = user.ID,
                email = user.Email,
                message = "User registered successfully"
            });
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}