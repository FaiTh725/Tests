using Authorization.Domain.Validators;
using CSharpFunctionalExtensions;

namespace Authorization.Domain.Entities
{
    public class User : Entity
    {
        public string UserName { get; set; }

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public Role Role { get; private set; }
        public string RoleId { get; private set; }

        // For EF
        public User() {}

        private User(
            string userName,
            string email,
            string passwordHash,
            Role role)
        { 
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }

        public static Result<User> Initialize(
            string userName,
            string email,
            string passwordHash,
            Role role)
        {
            if(string.IsNullOrWhiteSpace(userName))
            {
                return Result.Failure<User>("UserName is emprt or null");
            }

            if(string.IsNullOrEmpty(passwordHash))
            {
                return Result.Failure<User>("PasswordHash is empty or null");
            }

            if(!UserValidator.IsValidEmail(email))
            {
                return Result.Failure<User>("Email is invalid, must contains one letter and one number");
            }

            if(role is null)
            {
                return Result.Failure<User>("Role is null");
            }

            return Result.Success(new User(
                userName,
                email,
                passwordHash,
                role));
        }
    }
}
