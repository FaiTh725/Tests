using TestEntity = Test.Domain.Entities.Test;
using Test.Domain.Enums;
using FluentAssertions;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.Tests
{
    public class InitializeTestsTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteTestName_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "  ";
            var description = "some text for description";
            var profileId = 1;
            var testType = TestType.Timed;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var test = TestEntity.Initialize(name, description, profileId, 
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("Test name should be in range from " +
                    $"{TestValidator.MIN_NAME_LENGTH} to {TestValidator.MAX_NAME_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenOutsideOfBoundsName_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "q";
            var description = "some text for description";
            var profileId = 1;
            var testType = TestType.Timed;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("Test name should be in range from " +
                    $"{TestValidator.MIN_NAME_LENGTH} to {TestValidator.MAX_NAME_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenEmptyOrWhiteSpaceDescription_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "OOP";
            var description = "  ";
            var profileId = 1;
            var testType = TestType.Timed;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("Description is empty or " +
                    $"length greater than {TestValidator.MAX_DESCRIPTION_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenLessThenZeroTestDuration_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "OOP";
            var description = "123";
            var profileId = 1;
            var testType = TestType.Timed;
            var testDurationInminutes = -1;
            var isPublic = true;

            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("Test duration should be greater than " +
                    TestValidator.MIN_TEST_DURATION.ToString());
        }

        [Fact]
        public void Initialize_WhenProgressiveTypeAndDurationNotNull_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "OOP";
            var description = "123";
            var profileId = 1;
            var testType = TestType.Progressive;
            var testDurationInminutes = 15;
            var isPublic = true;


            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("If TestType has timed status than duration is required and " +
                    "if TestType is progressive duration should be null");
        }

        [Fact]
        public void Initialize_WhenTimedTypeAndDurationNull_ShouldReturnFailedResult()
        {
            // Arrange
            var name = "OOP";
            var description = "123";
            var profileId = 1;
            var testType = TestType.Timed;
            double? testDurationInminutes = null;
            var isPublic = true;


            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsFailure.Should().BeTrue();
            test.Error.Should().Be("If TestType has timed status than duration is required and " +
                    "if TestType is progressive duration should be null");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var name = "OOP";
            var description = "123";
            var profileId = 1;
            var testType = TestType.Timed;
            double? testDurationInminutes = 15;
            var isPublic = true;
            var expectedCreatedTime = DateTime.UtcNow;

            // Act
            var test = TestEntity.Initialize(name, description, profileId,
                testType, testDurationInminutes, isPublic);

            // Assert
            test.IsSuccess.Should().BeTrue();
            test.Value.Name.Should().Be(name);
            test.Value.Description.Should().Be(description);
            test.Value.ProfileId.Should().Be(profileId);
            test.Value.DurationInMinutes.Should().Be(testDurationInminutes);
            test.Value.IsPublic.Should().Be(isPublic);
            test.Value.CreatedTime.Should()
                .BeCloseTo(expectedCreatedTime, TimeSpan.FromMicroseconds(1000));
        }
    }
}
