using FluentAssertions;
using Test.Domain.Entities;

namespace Testing.Domain.UnitTests.OutboxMessages
{
    public class InitializeOutboxMessageTests
    {
        [Fact]
        public void Initialize_WhenIsNullOrEmptyType_ShouldReturnFailedResult()
        {
            // Arrange
            var type = string.Empty;
            var content = @"{
                        ""data"": 123,
                        }";
            
            // Act
            var outboxMessage = OutboxMessage.Initialize(type, content);
            
            // Assert
            outboxMessage.IsFailure.Should().BeTrue();
            outboxMessage.Error.Should().Be("Type is empty");
        }

        [Fact]
        public void Initialize_WhenIsNullOrEmptyPayload_ShouldReturnFailedResult()
        {
            // Arrange
            var type = "Test.API.Message";
            var content = "";

            // Act
            var outboxMessage = OutboxMessage.Initialize(type, content);

            // Assert
            outboxMessage.IsFailure.Should().BeTrue();
            outboxMessage.Error.Should().Be("Payload is empty");
        }

        [Fact]
        public void Initialize_CorrectAnswers_ShouldReturnSuccessResult()
        {
            // Arrange
            var type = "Test.API.Message";
            var content = @"{
                        ""data"": 123,
                        }";

            // Act
            var outboxMessage = OutboxMessage.Initialize(type, content);

            // Assert
            outboxMessage.IsSuccess.Should().BeTrue();
            outboxMessage.Value.Type.Should().Be(type);
            outboxMessage.Value.Payload.Should().Be(content);
        }
    }
}
