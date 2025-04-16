using CSharpFunctionalExtensions;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Profile : Entity
    {
        public string Email { get; private set; }

        public string Name { get; private set; }

        public List<long> CreatedTestsId { get; private set; }

        private Profile(
            string name,
            string email)
        {
            Name = name;
            Email = email;

            CreatedTestsId = new List<long>();
        }

        private Profile(
            string name,
            string email,
            List<long> createdTestsId):
            this(name, email)
        {
            CreatedTestsId = createdTestsId;
        }

        public static Result<Profile> Initialize(
            string name, 
            string email)
        {
            var validation = Validate(name, email);
            if (!validation.IsSuccess)
                return Result.Failure<Profile>(validation.Error);

            return Result.Success(new Profile(name, email));
        }

        public static Result<Profile> Initialize(
            string name, 
            string email, 
            List<long> createdTestsId)
        {
            var validation = Validate(name, email);
            if (!validation.IsSuccess)
                return Result.Failure<Profile>(validation.Error);

            return Result.Success(new Profile(name, email, createdTestsId));
        }

        private static Result Validate(
            string name, 
            string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Name is null or white space");

            if (string.IsNullOrWhiteSpace(email) || !ProfileValidator.IsValidEmail(email))
                return Result.Failure("Email is empty or invalid signature");

            return Result.Success();
        }
    }
}
