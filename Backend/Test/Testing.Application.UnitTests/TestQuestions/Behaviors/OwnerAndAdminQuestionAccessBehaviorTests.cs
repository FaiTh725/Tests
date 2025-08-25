using Application.Shared.Exceptions;
using FluentAssertions;
using MediatR;
using Moq;
using Test.Application.Behaviors;
using Test.Application.Commands.Question.DeleteQuestion;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.TestQuestions.Behaviors
{
    public class OwnerAndAdminQuestionAccessBehaviorTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly OwnerAndAdminQuestionAccessBehavior<DeleteQuestionCommand, string> behavior;
        private readonly Mock<RequestHandlerDelegate<string>> nextMock;

        public OwnerAndAdminQuestionAccessBehaviorTests()
        {
            unitOfWorkMock = new();
            questionRepositoryMock = new();
            testRepositoryMock = new();

            behavior = new OwnerAndAdminQuestionAccessBehavior<DeleteQuestionCommand, string>(unitOfWorkMock.Object);
            nextMock = new();

            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenQuestionDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new DeleteQuestionCommand
            { 
                OwnerId = 1,
                QuestionId = 1,
                Role = "User"
            };

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.OwnerId, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Question);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Question doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowInternalServerErrorException()
        {
            // Arrange
            var command = new DeleteQuestionCommand
            {
                OwnerId = 1,
                QuestionId = 1,
                Role = "User"
            };

            var existedQuestion = Question.Initialize("question?", 5, QuestionType.OneAnswer, 1).Value;
            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.OwnerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InternalServerErrorException>()
                .WithMessage("Invalid data in database");
        }

        [Fact]
        public async Task Handle_WhenProfileIsNotOwner_ShouldThrowForbiddenAccessException()
        {
            // Arrange
            var command = new DeleteQuestionCommand
            {
                OwnerId = 1,
                QuestionId = 1,
                Role = "User"
            };

            var existedQuestion = Question.Initialize("question?", 5, QuestionType.OneAnswer, 1).Value;
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, true).Value;
            
            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.OwnerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            // Act
            var act = async () => await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ForbiddenAccessException>()
                .WithMessage("Only the owner or an admin have access to the test");
        }

        [Fact]
        public async Task Handle_WhenUserIsAdmin_ShouldReturnNextResult()
        {
            // Arrange
            var expectedResult = "Something";
            var command = new DeleteQuestionCommand
            {
                OwnerId = 1,
                QuestionId = 1,
                Role = "Admin"
            };

            var existedQuestion = Question.Initialize("question?", 5, QuestionType.OneAnswer, 1).Value;
            var existedTest = TestEntity.Initialize("name", "description", 2, TestType.Timed, 15, true).Value;

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.OwnerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");
            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async Task Handle_WhenProfileIsOwner_ShouldReturnNextResult()
        {
            // Arrange
            var expectedResult = "Something";
            var command = new DeleteQuestionCommand
            {
                OwnerId = 1,
                QuestionId = 1,
                Role = "User"
            };

            var existedQuestion = Question.Initialize("question?", 5, QuestionType.OneAnswer, 1).Value;
            var existedTest = TestEntity.Initialize("name", "description", 1, TestType.Timed, 15, true).Value;

            questionRepositoryMock.Setup(x => x
                .GetQuestion(
                    command.OwnerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedQuestion);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            nextMock.Setup(x => x
                .Invoke(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("Something");
            // Act
            var result = await behavior.Handle(command, nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
