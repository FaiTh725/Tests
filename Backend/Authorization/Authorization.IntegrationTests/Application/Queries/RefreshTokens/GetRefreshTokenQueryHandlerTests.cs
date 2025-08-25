using Application.Shared.Exceptions;
using Authorization.Application.Contracts.RefreshToken;
using Authorization.Application.Contracts.User;
using Authorization.Application.Queries.RefreshTokenEntity.GetRefreshTokenWithUser;
using Authorization.Domain.Entities;
using FluentAssertions;

namespace Authorization.IntegrationTests.Application.Queries.RefreshTokens
{
    [Collection("Integration Tests")]
    public class GetRefreshTokenQueryHandlerTests :
        BaseIntegrationTest
    {
        public GetRefreshTokenQueryHandlerTests(
            CustomWebFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Handle_WhenRefreshTokenDoesntExist_ShouldThrowNotFound()
        {
            // Arrange
            var query = new GetRefreshTokenWithUserQuery 
            { 
                Token = "fsdfdssfhg"
            };

            // Act
            var act = async () => await sender.Send(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("RefreshToken doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenExists_ShouldReturnRefreshTokenWithOwner()
        {
            // Arrange
            var userEntity = User.Initialize(
                "test", "test@mail.com",
                "fvsdfgs3245", "User").Value;

            var userFromDb = await context.Users
                .AddAsync(userEntity);
            await context.SaveChangesAsync();

            var refreshToken = RefreshToken.Initialize(
                "fsdffdsgg", userFromDb.Entity,
                new DateTime(2030, 5, 10, 0, 0, 0, DateTimeKind.Utc)).Value;
            
            var refreshTokenFromDb = await context.RefreshTokens
                .AddAsync(refreshToken);
            await context.SaveChangesAsync();

            var expectedResult = new UserRefreshTokenResponse
            {
                User = new UserResponse
                {
                    Email = "test@mail.com",
                    Role = "User",
                    UserName = "test"
                },
                ExpireOn = new DateTime(2030, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                Id = refreshTokenFromDb.Entity.Id,
                Token = "fsdffdsgg"
            };

            var query = new GetRefreshTokenWithUserQuery
            {
                Token = "fsdffdsgg"
            };

            // Act
            var result = await sender.Send(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
