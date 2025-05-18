using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.Question;
using Test.Application.Contracts.QuestionAnswerEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.QuestionAnswerEntity.Specifications;
using Test.Application.Queries.QuestionEntity.Specifications;
using Test.Application.Queries.Test.GetTestToPass;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Queries
{
    public class GetTestToPassQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<ITestRepository> testRepositoryMock;
        private readonly Mock<IQuestionRepository> questionRepositoryMock;
        private readonly Mock<IQuestionAnswerRepository> answerRepositoryMock;
        private readonly Mock<IBlobService> blobServiceMock;
    
        private readonly GetTestToPassHandler handler;

        public GetTestToPassQueryHandlerTests()
        {
            unitOfWorkMock = new();
            testRepositoryMock = new();
            questionRepositoryMock = new();
            answerRepositoryMock = new();
            blobServiceMock = new();

            handler = new GetTestToPassHandler(
                unitOfWorkMock.Object, 
                blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.QuestionRepository)
                .Returns(questionRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.QuestionAnswerRepository)
                .Returns(answerRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetTestToPassQuery
            {
                Id = 1
            };

            testRepositoryMock.Setup(x => x
                .GetTest(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestExists_ShouldReturnTestWithQuestionsAndAnswers()
        {
            // Arrange
            var expectedResult = new TestToPassResponse
            {
                Id = 1,
                Description = "description",
                Name = "test",
                TestType = TestType.Timed.ToString(),
                Questions = new List<QuestionToPassTest>()
                { 
                    new QuestionToPassTest
                    {
                        Id = 1,
                        QuestionType = QuestionType.OneAnswer.ToString(),
                        TestQuestion = "question?",
                        Answers = new List<QuestionAnswerToPassTest>
                        {
                            new QuestionAnswerToPassTest
                            {
                                Id = 1,
                                Answer = "It is correct"
                            }
                        }
                    }
                }
            };

            var query = new GetTestToPassQuery
            {
                Id = 1
            };

            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;
            // set id
            var type = typeof(TestEntity);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedTest, [1]);

            var existedQuestion = Question.Initialize("question?", 5, QuestionType.OneAnswer, 1).Value;
            // set Id
            type = typeof(Question);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedQuestion, [1]);

            var existedAnswer = QuestionAnswer.Initialize("It is correct", true, 1).Value;
            // set Id
            type = typeof(QuestionAnswer);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedAnswer, [1]);

            testRepositoryMock.Setup(x => x
                .GetTest(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            questionRepositoryMock.Setup(x => x
                .GetQuestionsByCriteria(
                    It.IsAny<QuestionsByTestIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedQuestion]);

            answerRepositoryMock.Setup(x => x
                .GetQuestionAnswersByCriteria(
                    It.IsAny<AnswersByQuestionIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedAnswer]);

            // Act
            var testToPass = await handler.Handle(query, CancellationToken.None);

            // Assert
            testToPass.Should().BeEquivalentTo(expectedResult);
        }
    }
}
