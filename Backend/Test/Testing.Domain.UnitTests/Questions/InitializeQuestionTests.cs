using FluentAssertions;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Validators;

namespace Testing.Domain.UnitTests.Questions
{
    public class InitializeQuestionTests
    {
        [Fact]
        public void Initialize_WhenEmptyOrWhiteQuestionText_ShouldReturnFailedResult()
        {
            // Arrange
            var questionText = "   ";
            var questionWeight = 12;
            var questionType = QuestionType.OneAnswer;
            var testId = 1;

            // Act
            var question = Question.Initialize(questionText, questionWeight, 
                questionType, testId);

            // Assert
            question.IsFailure.Should().BeTrue();
            question.Error.Should().Be("Question is null or white space " +
                    $"or length outside of {QuestionValidator.MIN_QUESTION_LENGTH} - {QuestionValidator.MAX_QUESTION_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenLengthOutsideOfBounds_ShouldReturnFailedResult()
        {
            // Arrange
            var questionText = "a";
            var questionWeight = 12;
            var questionType = QuestionType.OneAnswer;
            var testId = 1;

            // Act
            var question = Question.Initialize(questionText, questionWeight,
                questionType, testId);

            // Assert
            question.IsFailure.Should().BeTrue();
            question.Error.Should().Be("Question is null or white space " +
                    $"or length outside of {QuestionValidator.MIN_QUESTION_LENGTH} - {QuestionValidator.MAX_QUESTION_LENGTH}");
        }

        [Fact]
        public void Initialize_WhenQuestionWeightLessThanZero_ShouldReturnFailedResult()
        {
            // Arrange
            var questionText = "Where is PAMELA";
            var questionWeight = -12;
            var questionType = QuestionType.OneAnswer;
            var testId = 1;

            // Act
            var question = Question.Initialize(questionText, questionWeight,
                questionType, testId);

            // Assert
            question.IsFailure.Should().BeTrue();
            question.Error.Should().Be($"Question weight less than {QuestionValidator.MIN_QUESTION_WEIGHT}");
        }

        [Fact]
        public void Initialize_CorrectParameters_ShouldReturnSuccessResult()
        {
            // Arrange
            var questionText = "Where is PAMELA";
            var questionWeight = 12;
            var questionType = QuestionType.OneAnswer;
            var testId = 1;

            // Act
            var question = Question.Initialize(questionText, questionWeight,
                questionType, testId);

            // Assert
            question.IsSuccess.Should().BeTrue();
            question.Value.TestQuestion.Should().Be(questionText);
            question.Value.QuestionWeight.Should().Be(questionWeight);
            question.Value.QuestionType.Should().Be(questionType);
            question.Value.TestId.Should().Be(testId);
        }
    }
}
