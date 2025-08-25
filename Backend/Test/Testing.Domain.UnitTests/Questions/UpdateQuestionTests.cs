using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.Questions
{
    public class UpdateQuestionTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteQuestionText_ShouldReturnFailedResult()
        {
            // Arrange
            var question = Question.Initialize("Where is PAMELA", 12,
                QuestionType.OneAnswer, 1);
            var questionText = string.Empty;
            var questionWeight = 5;

            // Act
            var updateResult = question.Value.Update(
                questionText, questionWeight);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Question is null or white space " +
                    $"or length outside of {QuestionValidator.MIN_QUESTION_LENGTH} - {QuestionValidator.MAX_QUESTION_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenLengthOutsideOfBounds_ShouldReturnFailedResult()
        {
            // Arrange
            var question = Question.Initialize("Where is PAMELA", 12,
                QuestionType.OneAnswer, 1);
            var questionText = "r";
            var questionWeight = 5;

            // Act
            var updateResult = question.Value.Update(
                questionText, questionWeight);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be("Question is null or white space " +
                    $"or length outside of {QuestionValidator.MIN_QUESTION_LENGTH} - {QuestionValidator.MAX_QUESTION_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenQuestionWeightLessThanZero_ShouldReturnFailedResult()
        {
            // Arrange
            var question = Question.Initialize("Where is PAMELA", 12,
                QuestionType.OneAnswer, 1);
            var questionText = "Where is PAMELA";
            var questionWeight = -12;

            // Act
            var updateResult = question.Value.Update(
                questionText, questionWeight);

            // Assert
            updateResult.IsFailure.Should().BeTrue();
            updateResult.Error.Should().Be($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var question = Question.Initialize("Where is PAMELA", 12,
                QuestionType.OneAnswer, 1);
            var questionText = "Where is PAMELA??";
            var questionWeight = 1;

            // Act
            var updateResult = question.Value.Update(
                questionText, questionWeight);

            // Assert
            updateResult.IsSuccess.Should().BeTrue();
            question.Value.TestQuestion.Should().Be(questionText);
            question.Value.QuestionWeight.Should().Be(questionWeight);
        }
    }
}
