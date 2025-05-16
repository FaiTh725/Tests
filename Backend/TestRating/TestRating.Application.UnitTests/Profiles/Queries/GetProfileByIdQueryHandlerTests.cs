using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.ProfileEntity.GetProfileById;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;
using TestRating.Domain.Repositories;

namespace TestRating.Application.UnitTests.Profiles.Queries
{
    public class GetProfileByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;

        private readonly GetProfileByIdHandler handler;

        public GetProfileByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();

            handler = new GetProfileByIdHandler(unitOfWorkMock.Object);
            
            unitOfWorkMock.Setup(uow => uow.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetProfileByIdQuery 
            { 
                Id = 1 
            };
            
            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);
            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExist_ShouldReturnProfile()
        {
            // Arrange
            var query = new GetProfileByIdQuery 
            { 
                Id = 1 
            };

            var existedProfile = Profile.Initialize("test@mail.ru", "FaITh").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);
            
            var expectedProfile = new BaseProfileResponse
            {
                Id = 1,
                Email = "test@mail.ru",
                Name = "FaITh"
            };

            profileRepositoryMock.Setup(x => x
                .GetProfileById(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            var profile = await handler.Handle(query, CancellationToken.None);

            // Assert
            profile.Should().BeEquivalentTo(expectedProfile);
        }
    }
}
