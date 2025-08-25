using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.TestAccesses
{
    public class InitializeTestAccessTests
    {
        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var testId = 1;
            var targetEntityId = 2;
            var targetEntityType = TargetAccessEntityType.Group;

            // Act
            var testAccess = TestAccess.Initialize(testId, targetEntityId, targetEntityType);

            // Assert
            testAccess.IsSuccess.Should().BeTrue();
            testAccess.Value.TestId.Should().Be(testId);
            testAccess.Value.TargetAccessEntityType.Should().Be(targetEntityType);
            testAccess.Value.TargetEntityId.Should().Be(targetEntityId);
        }
    }
}
