using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Commands.TestAccessEntity.LimitTestAccess;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.TestAccesses.Commands
{
    public class LimitTestAccessCommandHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestAccessRepository> testAccessRepositoryMock;

        private readonly LimitTestAccessHandler handler;

        public LimitTestAccessCommandHandlerTests()
        {
            unitOfWorkMock = new();
            testAccessRepositoryMock = new();

            handler = new LimitTestAccessHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.AccessRepository)
                .Returns(testAccessRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestAccessDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new LimitTestAccessCommand
            {
                TargetEntity = TargetAccessEntityType.Profile,
                TargetEntityId = 1,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId, 
                    command.TargetEntityId, 
                    command.TargetEntity, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestAccess);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Access doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestAccessExist_ShouldDeleteTestAccess()
        {
            // Arrange
            var command = new LimitTestAccessCommand
            {
                TargetEntity = TargetAccessEntityType.Profile,
                TargetEntityId = 1,
                TestId = 1,
                OwnerId = 1,
                Role = "User"
            };
            var existedTestAccess = TestAccess.Initialize(1, 1, TargetAccessEntityType.Profile).Value;

            testAccessRepositoryMock.Setup(x => x
                .GetTestAccess(
                    command.TestId,
                    command.TargetEntityId,
                    command.TargetEntity,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTestAccess);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert

            testAccessRepositoryMock.Verify(x => x
                .DeleteTestAccess(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
