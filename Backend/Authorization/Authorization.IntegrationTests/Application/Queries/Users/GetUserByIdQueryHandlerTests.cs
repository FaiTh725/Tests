using Application.Shared.Exceptions;
using Authorization.Application.Contracts.User;
using Authorization.Application.Queries.UserEntity.GetUserById;
using Authorization.Dal.Repositories;
using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.IntegrationTests.Application.Queries.Users
{
    [Collection("Integration Tests")]
    public class GetUserByIdQueryHandlerTests :
        BaseIntegrationTest
    {
        public GetUserByIdQueryHandlerTests(
            CustomWebFactory factory) :
            base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenUserDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetUserByIdQuery 
            { 
                Id = 1
            };

            // Act
            var act = async () => await sender.Send(query);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("User doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenUserExists_ShouldReturnsUserData()
        {
            // Arrange
            var expectedResult = new UserResponse 
            { 
                Email = "test@mail.com",
                Role = "User",
                UserName = "test"
            };

            var userEntity = User.Initialize(
                "test", "test@mail.com", 
                "fvsdfgs3245", "User").Value;

            var userDb = await context.Users
                .AddAsync(userEntity);
            await context.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                Id = userDb.Entity.Id
            };

            // Act
            var result = await sender.Send(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
