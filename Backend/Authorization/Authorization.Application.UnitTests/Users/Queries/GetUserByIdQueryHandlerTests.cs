using Application.Shared.Exceptions;
using Authorization.Application.Contracts.User;
using Authorization.Application.Queries.UserEntity.GetUserById;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Authorization.Application.UnitTests.Users.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IUserRepository> userRepositoryMock;

        private readonly GetUserByIdHandler handler;

        public GetUserByIdQueryHandlerTests()
        {
            unitOfWorkMock = new();
            userRepositoryMock = new();

            handler = new GetUserByIdHandler(unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handler_WhenUserDoesntExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };

            userRepositoryMock.Setup(x => x.GetUserById(
                    query.Id, 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("User doesnt exist");
        }

        [Fact]
        public async Task Handler_WhenUserExist_ShouldReturnUserFromDb()
        {
            // Arrange
            var query = new GetUserByIdQuery { Id = 1 };
            var userFromDb = User.Initialize("Faith", "string@mail.ru", "dfasfr214tgs", "User").Value;
            userRepositoryMock.Setup(x => x.GetUserById(
                    query.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userFromDb);

            unitOfWorkMock.Setup(x => x.UserRepository)
                .Returns(userRepositoryMock.Object);

            // Act
            var user = await handler.Handle(query, CancellationToken.None);

            // Assert
            user.Email.Should().Be(userFromDb.Email);
            user.Role.Should().Be(userFromDb.RoleId);
            user.UserName.Should().Be(userFromDb.UserName);
        }
    }
}
