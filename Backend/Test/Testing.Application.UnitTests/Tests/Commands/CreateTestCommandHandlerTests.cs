using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Test.CreateTest;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class CreateTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly CreateTestHandler handler;

        public CreateTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            testRepositoryMock = new();

            handler = new CreateTestHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new CreateTestCommand
            {
                IsPublic = true,
                Description = "some text",
                DurationInMinutes = 15,
                Name = "test",
                ProfileId = 1,
                TestType = TestType.Timed
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExist_ShouldAddTest()
        {
            // Arrange
            var command = new CreateTestCommand
            {
                IsPublic = true,
                Description = "some text",
                DurationInMinutes = 15,
                Name = "test",
                ProfileId = 1,
                TestType = TestType.Timed
            };
            var existedProfile = Profile.Initialize("FaiTh", "test@mail.com").Value;
            var newTestFromDb = TestEntity.Initialize("some text", "some text", 1, TestType.Timed, 15, true).Value;
            // set Id
            var type = typeof(TestEntity);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(newTestFromDb, [1]);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    command.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            testRepositoryMock.Setup(x => x
                .AddTest(
                    It.IsAny<TestEntity>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(newTestFromDb);

            // Act
            var testId = await handler.Handle(command, CancellationToken.None);

            // Assert
            testId.Should().Be(newTestFromDb.Id);

            testRepositoryMock.Verify(x => x
                .AddTest(
                    It.IsAny<TestEntity>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
