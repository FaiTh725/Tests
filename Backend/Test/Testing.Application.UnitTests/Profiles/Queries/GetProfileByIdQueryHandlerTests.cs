using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileById;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.Profiles.Queries
{
    public class GetProfileByIdQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;

        private readonly GetProfileByIdHandler handler;

        public GetProfileByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();

            handler = new GetProfileByIdHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetProfileByIdQuery
            {
                Id = 1,
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExists_ShouldReturnsProfileData()
        {
            // Arrange
            var query = new GetProfileByIdQuery
            {
                Id = 1,
            };
            var existedProfile = Profile.Initialize("test", "test@mail.com").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);

            var expectedProfileResponse = new ProfileResponse
            {
                Email = existedProfile.Email,
                Id = existedProfile.Id,
                Name = existedProfile.Name
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            var profileResponse = await handler.Handle(query, CancellationToken.None);

            // Assert
            profileResponse.Should().BeEquivalentTo(expectedProfileResponse);
        }
    }
}
