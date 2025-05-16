using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Commands.FeedbackEntity.SendFeedback;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.File;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Commands
{
    public class SendFeedbackCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;

        private readonly SendFeedbackHandler handler;

        private readonly Profile profile;

        public SendFeedbackCommandHandlerTests()
        {
            unitOfWorkMock = new();
            blobServiceMock = new();
            profileRepositoryMock = new();
            feedbackRepositoryMock = new();

            handler = new SendFeedbackHandler(
                unitOfWorkMock.Object, blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);

            profile = Profile.Initialize("zelenukho725@mail.com", "Faith").Value;
            // set profile Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(profile, [1]);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var command = new SendFeedbackCommand
            {
                ProfileId = 1
            };

            profileRepositoryMock.Setup(x => x
            .GetProfileById(
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
        public async Task Handle_CorrectCommand_ShouldAddFeedback()
        {
            // Arrange
            var command = new SendFeedbackCommand
            {
                ProfileId = 1,
                Rating = 5,
                TestId = 1,
                Text = "good!)"
            };
            var expectedFeedback = Feedback.Initialize("some text here", 1, 5, profile.Id).Value;

            profileRepositoryMock.Setup(x => x
            .GetProfileById(
                command.ProfileId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(profile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackByCriteria(
                    It.IsAny<FeedbackFromProfileToTestSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            feedbackRepositoryMock.Setup(x => x
                .AddFeedback(
                    It.IsAny<Feedback>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFeedback);

            // Act
            var feedbackId = await handler.Handle(command, CancellationToken.None);

            // Assert
            feedbackId.Should().Be(expectedFeedback.Id);
            unitOfWorkMock.Verify(x => x
                .SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            blobServiceMock.Verify(x => x
                .UploadBlobs(
                    It.IsAny<string>(), 
                    It.IsAny<List<FileModel>>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
            feedbackRepositoryMock.Verify(x => x
                .AddFeedback(
                    It.IsAny<Feedback>(), 
                    It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProfileAlreadySentFeedbackForThisTest_ShouldThrowConflicException()
        {
            // Arrange
            var command = new SendFeedbackCommand
            {
                ProfileId = 1,
                Rating = 5,
                TestId = 1,
                Text = "good!)"
            };
            var existedFeedback = Feedback.Initialize("good!)", 1, 5, profile.Id).Value;

            profileRepositoryMock.Setup(x => x
            .GetProfileById(
                command.ProfileId,
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(profile);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackByCriteria(
                    It.IsAny<FeedbackFromProfileToTestSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage("Feedback for this test has already sent, update old");
        }
    }
}
