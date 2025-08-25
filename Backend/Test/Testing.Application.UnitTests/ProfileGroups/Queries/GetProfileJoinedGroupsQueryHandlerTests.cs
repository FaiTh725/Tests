using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup;
using Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Entities;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;

namespace Testing.Application.UnitTests.ProfileGroups.Queries
{
    public class GetProfileJoinedGroupsQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<IProfileGroupRepository> groupRepositoryMock;

        private readonly GetProfileJoinedGroupHandler handler;

        public GetProfileJoinedGroupsQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            groupRepositoryMock = new();

            handler = new GetProfileJoinedGroupHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.ProfileGroupRepository)
                .Returns(groupRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowBadRequestException()
        {
            // Arrange
            var query = new GetProfileJoinedGroupQuery()
            {
                ProfileId = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExists_ShouldReturnProfileJoinedGroups()
        {
            // Arrange
            var expectedResult = new List<GroupInfo>()
            {
                new GroupInfo
                {
                    Id = 1,
                    Name = "test",
                },
                new GroupInfo
                {
                    Id = 2,
                    Name = "test1",
                }
            };

            var query = new GetProfileJoinedGroupQuery
            {
                ProfileId = 1
            };
            var existedProfile = Profile.Initialize("test", "test@mail.com").Value;

            var existedGroup = ProfileGroup.Initialize("test", 1).Value;
            var existedGroup1 = ProfileGroup.Initialize("test1", 1).Value;
            // set id
            var type = typeof(ProfileGroup);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedGroup1, [2]);
            method!.Invoke(existedGroup, [1]);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            groupRepositoryMock.Setup(x => x
                .GetProfileGroupsByCriteria(
                    It.IsAny<GroupsProfileJoinedSpecification>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedGroup, existedGroup1]);

            // Act
            var profileCreatedGroups = await handler.Handle(query, CancellationToken.None);

            // Assert
            profileCreatedGroups.Should().BeEquivalentTo(expectedResult);
        }
    }
}
