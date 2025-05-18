using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.GetGroupById;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Queries
{
    public class GetProfileGroupByIdQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly GetGroupByIdHandler handler;

        public GetProfileGroupByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            groupRepositoryMock = new();

            handler = new GetGroupByIdHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenGroupDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetGroupByIdQuery
            {
                Id = 1
            };

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as ProfileGroup);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Profile group doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenGroupOwnerDoesntExist_ShouldThrowInternalServerErrorException()
        {
            // Arrange
            var query = new GetGroupByIdQuery
            {
                Id = 1
            };
            var existedGroup = ProfileGroup.Initialize("name", 1).Value;

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InternalServerErrorException>()
                .WithMessage("Group doesnt have an owner");
        }

        [Fact]
        public async Task Handle_WhenQueryCorrect_ShouldReturnGroupWithOwner()
        {
            // Arrange
            var expectedResult = new GroupInfoWithOwner 
            {
                Id = 1,
                Name = "name",
                Owner = new ProfileResponse 
                { 
                    Email = "test@mail.com", 
                    Id = 1,
                    Name = "test"
                } 
            };

            var query = new GetGroupByIdQuery
            {
                Id = 1
            };
            var existedProfile = Profile.Initialize("test", "test@mail.com").Value;
            // set id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);

            var existedGroup = ProfileGroup.Initialize("name", 1).Value;
            // set id
            type = typeof(ProfileGroup);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedGroup, [1]);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroup(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedGroup);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            var profileGroupWithOwner = await handler.Handle(query, CancellationToken.None);

            // Assert
            profileGroupWithOwner.Should().BeEquivalentTo(expectedResult);
        }
    }
}
