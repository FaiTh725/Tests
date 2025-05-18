using Application.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.GetTestInfoById;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;
using Test.Domain.Repositories;
using TestEntity = Test.Domain.Entities.Test;

namespace Testing.Application.UnitTests.Tests.Queries
{
    public class GetTestInfoByIdQueryHandlerTests
    {
        private readonly Mock<INoSQLUnitOfWork> unitOfWorkMock;
        private readonly Mock<IProfileRepository> profileRepositoryMock;
        private readonly Mock<ITestRepository> testRepositoryMock;

        private readonly GetTestInfoByIdHandler handler;

        public GetTestInfoByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            profileRepositoryMock = new();
            testRepositoryMock = new();

            handler = new GetTestInfoByIdHandler(unitOfWorkMock.Object);

            unitOfWorkMock.Setup(x => x.ProfileRepository)
                .Returns(profileRepositoryMock.Object);
            unitOfWorkMock.Setup(x => x.TestRepository)
                .Returns(testRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenTestDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetTestInfoByIdQuery
            {
                Id = 1
            };

            testRepositoryMock.Setup(x => x
                .GetTest(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as TestEntity);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Test doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenTestDoesntHasOwner_ShouldThrowInternalServerErrorException()
        {
            // Arrange
            var query = new GetTestInfoByIdQuery
            {
                Id = 1
            };
            var existedTest = TestEntity.Initialize("test", "description", 1, TestType.Timed, 15, true).Value;

            testRepositoryMock.Setup(x => x
                .GetTest(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

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
                .WithMessage("Test without owner");
        }

        [Fact]
        public async Task Handle_WhenTestExists_ShouldReturnTestInfo()
        {
            // Arrange
            var expectedResult = new TestInfo
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
            };

            var query = new GetTestInfoByIdQuery
            {
                Id = 1
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

            testRepositoryMock.Setup(x => x
                .GetTest(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedTest);

            profileRepositoryMock.Setup(x => x
                .GetProfile(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existedProfile);

            // Act
            var testInfo = await handler.Handle(query, CancellationToken.None);

            // Assert
            testInfo.Should().BeEquivalentTo(expectedResult);
        }
    }
}
