using FluentAssertions;
using Test.Domain.Entities;

namespace Testing.Domain.UnitTests.ProfileAnswers
{
    public class InitializeProfileAnswerTests
    {
        [Fact]
        public void Initialize_WhenIsNullQuestionAnswers_ShouldReturnFailedResult()
        {
            // Arrange
            var sessionId = 1;
            var questionId = 1;
            List<long> questionAnswers = null;
            var isCorrect = false;

            // Act
            var profileAnswer = ProfileAnswer.Initialize(
                sessionId, questionId,
                questionAnswers, isCorrect);

            // Assert
            profileAnswer.IsFailure.Should().BeTrue();
            profileAnswer.Error.Should().Be("AnswersId is null");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var sessionId = 1;
            var questionId = 1;
            var questionAnswers = new List<long> { 1, 2, 3};
            var isCorrect = false;

            // Act
            var profileAnswer = ProfileAnswer.Initialize(
                sessionId, questionId,
                questionAnswers, isCorrect);

            // Assert
            profileAnswer.IsSuccess.Should().BeTrue();
            profileAnswer.Value.QuestionId.Should().Be(questionId);
            profileAnswer.Value.SessionId.Should().Be(sessionId);
            profileAnswer.Value.IsCorrect.Should().Be(isCorrect);
            profileAnswer.Value.QuestionAnswersId.Should()
                .BeEquivalentTo(questionAnswers);
        }
    }
}
