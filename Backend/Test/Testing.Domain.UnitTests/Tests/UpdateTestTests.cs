using FluentAssertions;
using Test.Domain.Enums;
using Test.Domain.Validators;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Domain.UnitTests.Tests
{
    public class UpdateTestTests
    {
        [Fact]
        public void Update_WhenEmptyOrWhiteTestName_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = string.Empty;
            var description = "some text for description";
            var testType = TestType.Timed;
            var  testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Test name should be in range from " +
                    $"{TestValidator.MIN_NAME_LENGTH} to {TestValidator.MAX_NAME_LENGTH}");
        }

        [Fact]
        public void Update_WhenOutsideOfBoundsName_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "q";
            var description = "some text for description";
            var testType = TestType.Timed;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Test name should be in range from " +
                    $"{TestValidator.MIN_NAME_LENGTH} to {TestValidator.MAX_NAME_LENGTH}");
        }

        [Fact]
        public void Update_WhenEmptyOrWhiteSpaceDescription_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "OOP";
            var description = "";
            var testType = TestType.Timed;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Description is empty or " +
                    $"length greater than {TestValidator.MAX_DESCRIPTION_LENGTH}");
        }

        [Fact]
        public void Update_WhenLessThenZeroTestDuration_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "OOP";
            var description = "some text for description";
            var testType = TestType.Timed;
            var testDurationInminutes = -1;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Test duration should be greater than " +
                    TestValidator.MIN_TEST_DURATION.ToString());
        }

        [Fact]
        public void Update_WhenProgressiveTypeAndDurationNotNull_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "OOP";
            var description = "123";
            var testType = TestType.Progressive;
            var testDurationInminutes = 15;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("If TestType has timed status than duration is required and " +
                    "if TestType is progressive duration should be null");
        }

        [Fact]
        public void Update_WhenTimedTypeAndDurationNull_ShouldReturnFailedResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "OOP";
            var description = "123";
            var testType = TestType.Timed;
            double? testDurationInminutes = null;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("If TestType has timed status than duration is required and " +
                    "if TestType is progressive duration should be null");
        }

        [Fact]
        public void Update_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var test = TestEntity.Initialize("OOP", "some text for description", 1,
                TestType.Timed, 15, true);
            var name = "SOLID";
            var description = "what is solid works";
            var testType = TestType.Progressive;
            double? testDurationInminutes = null;
            var isPublic = true;

            // Act
            var updateResult = test.Value.Update(name,
                                description,
                                isPublic,
                                testType,
                                testDurationInminutes);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();
            test.Value.Name.Should().Be(name);
            test.Value.Description.Should().Be(description);
            test.Value.TestType.Should().Be(testType);
            test.Value.DurationInMinutes.Should().Be(testDurationInminutes);
            test.Value.IsPublic.Should().Be(isPublic);
        }
    }
}
