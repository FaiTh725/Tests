using System.Text.RegularExpressions;

namespace Authorization.Domain.Validators
{
    public class UserValidator
    {
        public static int MAX_PASSWORD_LENGHT = 30;
        public static int MIN_PASSWORD_LENGHT = 5;

        public static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return emailRegex.IsMatch(email);
        }

        public static bool IsValidPassword(string password)
        {
            if(password.Length < MIN_PASSWORD_LENGHT ||
                password.Length > MAX_PASSWORD_LENGHT)
            {
                return false;
            }

            var passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d).+$");

            return passwordRegex.IsMatch(password);
        }
    }
}
