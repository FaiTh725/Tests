using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.GetProfileTests;
using Test.Application.Queries.Test.Specifications;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Queries
{
    public class GetProfileTestsQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly GetProfileTestsHandler handler;

        public GetProfileTestsQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            testRepositoryMock = new();

            handler = new GetProfileTestsHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenProfileDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetProfileTestsQuery
            {
                ProfileId = 1
            };

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    query.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as Profile);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Profile doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenProfileExists_ShouldReturnProfileTests()
        {
            // Arrange
            var expectedResult = new List<TestInfo>
            {
                new TestInfo
                {
                    Id = 1,
                    Description = "description",
                    IsPublic = true,
                    Name = "test",
                    TestType = TestType.Timed.ToString(),
                    DurationInMinutes = 15,
                    CreatedTime = new DateTime(2025, 5, 10),
                    Owner = new ProfileResponse
                    {
                        Id = 1,
                        Name = "name",
                        Email = "test@mail.com"
                    }
                }
            };
            
            var query = new GetProfileTestsQuery
            {
                ProfileId = 1
            };

            var existedProfile = Profile.Initialize("name", "test@mail.com").Value;
            // set Id
            var type = typeof(Profile);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(existedProfile, [1]);

            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;
            // set id
            type = typeof(TestEntity);
            property = type.GetProperty("Id");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedTest, [1]);
            // set CreatedTime
            type = typeof(TestEntity);
            property = type.GetProperty("CreatedTime");
            method = property!.GetSetMethod(true);
            method!.Invoke(existedTest, [new DateTime(2025, 5, 10)]);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    query.ProfileId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            testRepositoryMock.Setup(x => x
                .GetTestsByCriteria(
                    It.IsAny<TestsByProfileIdSpecification>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([existedTest]);

            // Act
            var tests = await handler.Handle(query, CancellationToken.None);

            // Assert
            tests.Should().BeEquivalentTo(expectedResult);
        }
    }
}
