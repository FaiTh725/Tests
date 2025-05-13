using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.QuestionAnswers
{
    public class InitializeQuestionAnswerTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteQuestionAnswer_ShouldReturnFailedResult()
        {
            // Arrange
            var answer = "  ";
            var isCorrect = false;
            var questionId = 1;

            // Act
            var questionAnswer = QuestionAnswer.Initialize(answer, isCorrect, questionId);

            // Assert
            questionAnswer.IsFailure.Should().BeTrue();
            questionAnswer.Error.Should().Be("Answer is null or white space " +
                    $"or length outside from range {QuestionAnswerValidator.MIN_ANSWER_LENGTH} - {QuestionAnswerValidator.MAX_ANSWER_LENGTH}");
            
        }

        [Fact]
        public void Initialize_WhenOutsideOfBoundsLengthQuestionAnswer_ShouldReturnFailedResult()
        {
            // Arrange
            var answer = "a";
            var isCorrect = false;
            var questionId = 1;

            // Act
            var questionAnswer = QuestionAnswer.Initialize(answer, isCorrect, questionId);

            // Assert
            questionAnswer.IsFailure.Should().BeTrue();
            questionAnswer.Error.Should().Be("Answer is null or white space " +
                    $"or length outside from range {QuestionAnswerValidator.MIN_ANSWER_LENGTH} - {QuestionAnswerValidator.MAX_ANSWER_LENGTH}");

        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var answer = "is correct answer";
            var isCorrect = true;
            var questionId = 1;

            // Act
            var questionAnswer = QuestionAnswer.Initialize(answer, isCorrect, questionId);

            // Assert
            questionAnswer.IsSuccess.Should().BeTrue();
            questionAnswer.Value.Answer.Should().Be(answer);
            questionAnswer.Value.IsCorrect.Should().Be(isCorrect);
            questionAnswer.Value.QuestionId.Should().Be(questionId);
        }
    }
}
