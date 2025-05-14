using Application.Shared.Exceptions;
using Authorization.Application.Queries.RefreshTokenEntity.GetRefreshTokenWithUser;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace Authorization.Application.UnitTests.RefreshTokens.Queries
{
    public class GetRefreshTokenWithUserQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;

        private readonly GetRefreshTokenWithUserHandler handler;

        public GetRefreshTokenWithUserQueryHandlerTests()
        {
            unitOfWorkMock = new();
            refreshTokenRepositoryMock = new();

            handler = new GetRefreshTokenWithUserHandler(unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenRefreshTokenDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetRefreshTokenWithUserQuery { Token = "efew21rtgsd" };

            refreshTokenRepositoryMock.Setup(x => x.GetRefreshTokenWithUser(
                    query.Token, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as RefreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("RefreshToken doesnt exist");
        }

        [Fact]
        public async Task Handle_WhenRefreshTokenExist_ShouldReturnRefreshTokenWithUser()
        {
            // Arrange
            var query = new GetRefreshTokenWithUserQuery { Token = "efew21rtgsd" };
            var user = User.Initialize("FaiTh", "zelenukho725@mail.ru", "dfs4214g", "User").Value;
            var refreshToken = RefreshToken.Initialize(
                    "efew21rtgsd", user, 
                    DateTime.UtcNow.AddDays(1)).Value;
            // set refresh token id instead db
            var type = typeof(RefreshToken);
            var property = type.GetProperty("Id");
            var method = property!.GetSetMethod(true);
            method!.Invoke(refreshToken, [1]);

            refreshTokenRepositoryMock.Setup(x => x.GetRefreshTokenWithUser(
                    query.Token,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(refreshToken);

            unitOfWorkMock.Setup(x => x.RefreshTokenRepository)
                .Returns(refreshTokenRepositoryMock.Object);

            // Act
            var refreshTokenDb = await handler.Handle(query, CancellationToken.None);

            // Assert
            refreshTokenDb.Id.Should().Be(refreshToken.Id);
            refreshTokenDb.ExpireOn.Should().Be(refreshToken.ExpireOn);
            refreshTokenDb.Token.Should().Be(refreshToken.Token);
            refreshTokenDb.User.Email.Should().Be(user.Email);
            refreshTokenDb.User.Role.Should().Be(user.RoleId);
            refreshTokenDb.User.UserName.Should().Be(user.UserName);
        }
    }
}
