using CSharpFunctionalExtensions;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Test : Entity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public bool IsPublic { get; private set; }

        public TestType TestType { get; private set; }

        public long ProfileId { get; private set; }

        public List<long> QuestionsId { get ; private set; }

        private Test(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic = true)
        {
            Name = name;
            Description = description;
            ProfileId = profileId;
            TestType = testType;
            IsPublic = isPublic;

            CreatedTime = DateTime.UtcNow;
            QuestionsId = new List<long>();
        }

        private Test(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic,
            List<long> questionId):
            this(name, description, profileId, testType)
        {
            QuestionsId = questionId;
        }

        public static Result<Test> Initialize(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic = true)
        {
            var isValidate = Validate(name, description, 
                profileId, testType, isPublic);

            if(isValidate.IsFailure)
            {
                return Result.Failure<Test>(isValidate.Error);
            }

            return Result.Success(new Test(
                name,
                description,
                profileId,
                testType,
                isPublic));
        }

        public static Result<Test> Initialize(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic,
            List<long> questionId)
        {
            var isValidate = Validate(name, description,
                profileId, testType, isPublic);

            if (isValidate.IsFailure)
            {
                return Result.Failure<Test>(isValidate.Error);
            }

            if(questionId is null)
            {
                return Result.Failure<Test>("QuestionId is null");
            }

            return Result.Success(new Test(
                name,
                description,
                profileId,
                testType,
                isPublic,
                questionId));
        }

        private static Result Validate(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic = true)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name.Length < TestValidator.MIN_NAME_LENGHT ||
                name.Length > TestValidator.MAX_NAME_LENGHT)
            {
                return Result.Failure("Test name should be in range from " +
                    $"{TestValidator.MIN_NAME_LENGHT} to {TestValidator.MAX_NAME_LENGHT}");
            }

            if (string.IsNullOrWhiteSpace(description) ||
                description.Length > TestValidator.MAX_DESCRIPTION_LENGHT)
            {
                return Result.Failure("Description is empty or " +
                    $"lenght greater than {TestValidator.MAX_DESCRIPTION_LENGHT}");
            }

            return Result.Success();
        }
    }
}
