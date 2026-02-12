using System.Text.RegularExpressions;

namespace GymTime.Models.Services
{
    public class PasswordAuthenticator
    {
        public static (bool isValid, string? errorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password is required");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Password must contain at least one number");

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return (false, "Password must contain at least one special character");

            return (true, null);
        }
    }
}