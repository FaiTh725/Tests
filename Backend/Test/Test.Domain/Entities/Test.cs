using CSharpFunctionalExtensions;
using Test.Domain.Enums;
using Test.Domain.Events;
using Test.Domain.Primitives;
using Test.Domain.Validators;

namespace Test.Domain.Entities
{
    public class Test : DomainEventEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedTime { get; private set; }

        public bool IsPublic { get; private set; }

        public TestType TestType { get; private set; }

        public long ProfileId { get; private set; }

        public double? DurationInMinutes { get; private set; }

        private Test(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic = true,
            double? durationInMinutes = null)
        {
            Name = name;
            Description = description;
            ProfileId = profileId;
            TestType = testType;
            IsPublic = isPublic;

            CreatedTime = DateTime.UtcNow;
            DurationInMinutes = durationInMinutes;
        }

        public void Delete()
        {
            RaiseDomainEvent(new TestDeletedEvent(Id));
        }

        public Result Update(
            string name,
            string description,
            bool isPublic,
            TestType testType,
            double durationInMinutes)
        {
            var isValid = Validate(
                name,
                description,
                testType,
                durationInMinutes);

            if (isValid.IsFailure)
            {
                return Result.Failure(isValid.Error);
            }

            Name = name;
            Description = description;
            IsPublic = isPublic;

            return Result.Success();
        }

        public static Result<Test> Initialize(
            string name,
            string description,
            long profileId,
            TestType testType,
            bool isPublic = true,
            double? durationInMinutes = null)
        {
            var isValid = Validate(
                name,
                description,
                testType,
                durationInMinutes);

            if(isValid.IsFailure)
            {
                return Result.Failure<Test>(isValid.Error);
            }

            return Result.Success(new Test(
                name,
                description,
                profileId,
                testType,
                isPublic));
        }

        private static Result Validate(
            string name,
            string description,
            TestType testType,
            double? durationInMinutes = null)
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

            if(testType == TestType.Timed &&
                durationInMinutes == null)
            {
                return Result.Failure("If TestType has timed status than duration is requred");
            }

            if(durationInMinutes < TestValidator.MIN_TEST_DURATION)
            {
                return Result.Failure("Test duration should be greater than " +
                    TestValidator.MIN_TEST_DURATION.ToString());
            }

            return Result.Success();
        }
    }
}
