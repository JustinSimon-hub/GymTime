using GymTime.Models;
using GymTime.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GymTime.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _users;
        
        public AccountController(UserRepository users)
        {
            _users = users;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string email, string password)
        {
           
            var (isValid, errorMessage) = PasswordAuthenticator.ValidatePassword(password);
            if (!isValid)
            {
                ViewBag.Error = errorMessage;
                return View();
            }

            // Check if email already exists
            var existingUser = _users.GetByEmail(email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email is already registered.";
                return View();
            }

            _users.Register(email, password);
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _users.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.ID);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
