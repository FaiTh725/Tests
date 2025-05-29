using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Profile : Entity
    {
        public string Email { get; private set; }

        public string Name { get; private set; }

        private Profile(
            string name,
            string email)
        {
            Name = name;
            Email = email;
        }

        public static Result<Profile> Initialize(
            string name, 
            string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Profile>("Name is null or white space");

            if (string.IsNullOrWhiteSpace(email) || !ProfileValidator.IsValidEmail(email))
                return Result.Failure<Profile>("Email is empty or invalid signature");

            return Result.Success(new Profile(
                name, email));
        }
    }
}
