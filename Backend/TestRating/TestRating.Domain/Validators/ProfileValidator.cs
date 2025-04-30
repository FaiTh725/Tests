using System.Text.RegularExpressions;

namespace TestRating.Domain.Validators
{
    public static class ProfileValidator
    {
        public static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return emailRegex.IsMatch(email);
        }
    }
}
