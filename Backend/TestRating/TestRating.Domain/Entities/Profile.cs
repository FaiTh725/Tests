using CSharpFunctionalExtensions;
using TestRating.Domain.Validators;

namespace TestRating.Domain.Entities
{
    public class Profile : Entity
    {
        public string Email { get; private set; }
    
        public string Name { get; private set; }

        public List<Feedback> FeedBacks { get; private set; }

        public List<FeedbackReview> Reviews { get; private set; }

        public Profile(){}

        private Profile(
            string email,
            string name)
        {
            Email = email;
            Name = name;

            FeedBacks = new ();
            Reviews = new ();
        }

        public static Result<Profile> Initialize(
            string email,
            string name)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return Result.Failure<Profile>("Name is null or empty");
            }

            if(string.IsNullOrEmpty(email) ||
                ProfileValidator.IsValidEmail(email))
            {
                return Result.Failure<Profile>("Email is empty or invalid signature");
            }

            return Result.Success(new Profile(
                email,
                name));
        }
    }
}
