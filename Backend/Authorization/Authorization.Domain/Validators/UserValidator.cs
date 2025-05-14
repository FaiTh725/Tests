using System.Text.RegularExpressions;

namespace Authorization.Domain.Validators
{
    public class UserValidator
    {
        public const int MAX_PASSWORD_LENGTH = 30;
        public const int MIN_PASSWORD_LENGTH = 5;

        public static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return emailRegex.IsMatch(email);
        }

        public static bool IsValidPassword(string password)
        {
            if(password.Length < MIN_PASSWORD_LENGTH ||
                password.Length > MAX_PASSWORD_LENGTH)
            {
                return false;
            }

            var passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d).+$");

            return passwordRegex.IsMatch(password);
        }
    }
}
