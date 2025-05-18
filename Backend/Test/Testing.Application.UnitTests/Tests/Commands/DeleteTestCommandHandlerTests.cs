using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.Test.DeleteTest;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Primitives;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Commands
{
    public class DeleteTestCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly DeleteTestHandler handler;

        public DeleteTestCommandHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();

            handler = new DeleteTestHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteTestCommand
            {
                TestId = 1,
                OwnerId = 1,
                Role = "User"
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
        public async Task Handle_WhenTestExists_ShouldDeleteTest()
        {
            // Arrange
            var command = new DeleteTestCommand
            {
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTest = TestEntity.Initialize("some text", "test", 1, TestType.Timed, 15, true).Value;
            // set id
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
            await handler.Handle(command, CancellationToken.None);

            // Assert
            testRepositoryMock.Verify(x => x
                .DeleteTest(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);

            unitOfWorkMock.Verify(x => x
                .TrackEntity(
                    It.IsAny<DomainEventEntity>()), 
                Times.Once);
        }
    }
}
