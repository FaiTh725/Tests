using Authorization.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Authorization.IntegrationTests.Dal.Repositories
{
    public class UserRepositoryTests : 
        BaseIntegrationTest
    {
        public UserRepositoryTests(CustomWebFactory factory) : 
            base(factory)
        {}

        [Fact]
        public async Task AddUser_ShouldAddNewUserToDb()
        {
            // Arrange
            var user = User.Initialize("test", "test@mail.com", "fsdgwer325", "User").Value;

            // Act
            await unitOfWork.UserRepository
                .AddUser(user);
            await unitOfWork.SaveChangesAsync();

            // Assert
            var userFromDb = await context.Users
                .FirstOrDefaultAsync(x => x.Email == user.Email);
            userFromDb.Should().NotBeNull();
            userFromDb.UserName.Should().Be(user.UserName);
            userFromDb.PasswordHash.Should().Be(user.PasswordHash);
            userFromDb.Email.Should().Be(user.Email);
            userFromDb.RoleId.Should().Be(user.RoleId);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnsUser()
        {
            // Arrange
            var user = User.Initialize("test", "test@mail.com", "fsdgwer325", "User").Value;
            await unitOfWork.UserRepository
                .AddUser(user);
            await unitOfWork.SaveChangesAsync();

            // Act
            var userByEmail = await unitOfWork.UserRepository
                .GetUserByEmail(user.Email);

            // Assert
            userByEmail.Should().NotBeNull();
            userByEmail.UserName.Should().Be(user.UserName);
            userByEmail.PasswordHash.Should().Be(user.PasswordHash);
            userByEmail.Email.Should().Be(user.Email);
            userByEmail.RoleId.Should().Be(user.RoleId);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnsUser()
        {
            // Arrange
            var user = User.Initialize("test", "test@mail.com", "fsdgwer325", "User").Value;
            var userFromDb = await unitOfWork.UserRepository
                .AddUser(user);
            await unitOfWork.SaveChangesAsync();

            // Act
            var userByEmail = await unitOfWork.UserRepository
                .GetUserById(userFromDb.Id);

            // Assert
            userByEmail.Should().NotBeNull();
            userByEmail.UserName.Should().Be(user.UserName);
            userByEmail.PasswordHash.Should().Be(user.PasswordHash);
            userByEmail.Email.Should().Be(user.Email);
            userByEmail.RoleId.Should().Be(user.RoleId);
        }
    }
}
