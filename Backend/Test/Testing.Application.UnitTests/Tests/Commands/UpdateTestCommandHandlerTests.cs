using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Test.UpdateTest;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class UpdateTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly UpdateTestHandler handler;

        public UpdateTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();

            handler = new UpdateTestHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task When_TestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new UpdateTestCommand
            {
                TestId = 1
            };

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task When_TestExists_ShouldUpdateTest()
        {
            // Arrange
            var command = new UpdateTestCommand
            {
                TestId = 1,
                Description = "new description",
                DurationInMinutes = 14,
                IsPublic = true,
                Name = "new name",
                OwnerId = 1,
                Role = "User",
                TestType = TestType.Timed
            };
            var existedTest = TestEntity.Initialize("old text", "old description", 1, TestType.Timed, 15, false).Value;
            // set Id
            var type = typeof(TestEntity);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedTest, [1]);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    command.TestId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            // Act
            var testId = await handler.Handle(command, CancellationToken.None);

            // Assert
            testId.Should().Be(existedTest.Id);
            existedTest.Name.Should().Be(command.Name);
            existedTest.Description.Should().Be(command.Description);
            existedTest.TestType.Should().Be(command.TestType);
            existedTest.DurationInMinutes.Should().Be(command.DurationInMinutes);
            existedTest.IsPublic.Should().Be(command.IsPublic);
        }
    }
}
