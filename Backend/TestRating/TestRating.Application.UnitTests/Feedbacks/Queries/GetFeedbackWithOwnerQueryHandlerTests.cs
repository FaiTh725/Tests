using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Feedback;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner;
using TestRating.Application.Queries.FeedbackEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Feedbacks.Queries
{
    public class GetFeedbackWithOwnerQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<IFeedbackRepository> feedbackRepositoryMock;

        private readonly GetFeedbackWithOwnerHandler handler;

        public GetFeedbackWithOwnerQueryHandlerTests()
        {
            unitOfWorkMock = new();
            blobServiceMock = new();
            feedbackRepositoryMock = new();

            handler = new GetFeedbackWithOwnerHandler(
                unitOfWorkMock.Object, 
                blobServiceMock.Object);

            unitOfWorkMock.Setup(x => x.FeedbackRepository)
                .Returns(feedbackRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenFeedbackDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new GetFeedbackWithOwnerQuery
            {
                Id = 1
            };

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackByCriteria(
                    It.IsAny<FeedbackByIdWithOwnerSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Feedback);

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Feedback doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenFeedbackExist_ShouldReturnFeedbackWithOwner()
        {
            // Arrange
            var command = new GetFeedbackWithOwnerQuery
            {
                Id = 1
            };
            var expectedFeedback = new FeedbackResponse
            {
                Profile = new BaseProfileResponse
                {
                    Id = 1,
                    Email = "test@mail.ru",
                    Name = "FaiTh"
                },
                Id = 1,
                Rating = 5,
                SendTime = new DateTime(2025, 5, 10),
                TestId = 1,
                Text = "some text here",
                UpdateTime = new DateTime(2025, 5, 11)
            };

            var existedOwner = Profile.Initialize("test@mail.ru", "FaiTh").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedOwner, [1]);
            
            var existedFeedback = Feedback.Initialize("some text here", 1, 5, 1).Value;
            // set id
            type = typeof(Feedback);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [1]);
            // set owner
            type = typeof(Feedback);
            property = type.GetProperty("Owner");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [existedOwner]);
            // set sendTime
            type = typeof(Feedback);
            property = type.GetProperty("SendTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [new DateTime(2025, 5, 10)]);
            // set updateTime
            type = typeof(Feedback);
            property = type.GetProperty("UpdateTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedFeedback, [new DateTime(2025, 5, 11)]);

            feedbackRepositoryMock.Setup(x => x
                .GetFeedbackByCriteria(
                    It.IsAny<FeedbackByIdWithOwnerSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedFeedback);

            // Act
            var feedback = await handler.Handle(command, CancellationToken.None);

            // Assert
            feedback.Should().BeEquivalentTo(expectedFeedback);
        }
    }
}
