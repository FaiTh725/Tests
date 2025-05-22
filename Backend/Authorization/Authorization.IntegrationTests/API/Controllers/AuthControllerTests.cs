using Authorization.Application.Commands.Email.SendConfirmCode;
using Authorization.Application.Commands.Email.VerifyCode;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Authorization.IntegrationTests.API.Controllers
{
    public class AuthControllerTests : 
        BaseIntegrationTest
    {

        public AuthControllerTests(CustomWebFactory factory) :
            base(factory)
        {
        }

        [Fact]
        public async Task Login_WhenEmailIsntRegistered_ShouldReturns400Status()
        {
            // Arrange
            var request = new LoginCommand()
            {
                Email = "test@mail.com",
                Password = "string123"
            };

            // Act
            var httpResponse = await client.GetAsync($"/api/Auth/Login?email={request.Email}&password={request.Password}");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WhenInvalidCredentials_ShouldReturns400Status()
        {
            // Arrange
            var request = new LoginCommand()
            {
                Email = "test@mail.com",
                Password = "string123"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + request.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var httpResponse = await client.GetAsync($"/api/Auth/Login?email={request.Email}&password={request.Password}");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WhenInvalidRequest_ShouldReturns400Status()
        {
            // Arrange
            var request = new LoginCommand()
            {
                Email = "test",
                Password = "string123"
            };

            // Act
            var httpResponse = await client.GetAsync($"/api/Auth/Login?email={request.Email}&password={request.Password}");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await httpResponse.Content.ReadAsStringAsync();
            var validationDetail = JsonSerializer.Deserialize<ValidationProblemDetails>(body);
            validationDetail!.Errors.Should().ContainKey("Email");
        }

        [Fact]
        public async Task Login_WhenValidRequest_ShouldReturns200StatusAndSetCookies()
        {
            // Arrange
            var request = new LoginCommand()
            {
                Email = "test@mail.com",
                Password = "string321"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + request.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var httpResponse = await client.GetAsync($"/api/Auth/Login?email={request.Email}&password={request.Password}");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var cookies = httpResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            cookies.Should().Contain(x => x.StartsWith("token="));
            cookies.Should().Contain(x => x.StartsWith("refresh_token="));

            var existedUser = await context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            var userRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == existedUser!.Id);
            userRefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task Register_WhenEmailUnconfirmed_ShouldReturns403Status()
        {
            // Arrange
            var request = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string123",
                UserName = "test"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/Register", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Register_WhenInvalidCredentials_ShouldReturns400Status()
        {
            // Arrange
            var request = new RegisterCommand
            {
                Email = "testmail.com",
                Password = "string123",
                UserName = "test"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/Register", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WhenEmailAlreadyRegistered_ShouldReturns409Status()
        {
            // Arrange
            var request = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string123",
                UserName = "test"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + request.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/Register", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Register_WhenRequestIsCorrect_ShouldReturns200StatusAndSetCookies()
        {
            // Arrange
            var request = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string123",
                UserName = "test"
            };
            await cache.SetData("confirmed_email:" + request.Email, "Confirmed", 600);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/Register", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var cookies = httpResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            cookies.Should().Contain(x => x.StartsWith("token="));
            cookies.Should().Contain(x => x.StartsWith("refresh_token="));

            var registeredUser = await context.Users
                .FirstOrDefaultAsync(x => x.Email == request.Email);
            registeredUser.Should().NotBeNull();

            var userRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == registeredUser!.Id);
            userRefreshToken.Should().NotBeNull();
        }

        [Fact]
        public async Task SendEmailConfirmationCode_WhenEmailAlreadyRegistered_ShouldReturns409Status()
        {
            // Arrange
            var request = new SendConfirmCodeCommand
            {
                Email = "test@mail.com"
            };
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + request.Email, "Confirmed", 600);
            await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromSeconds(65));
                httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            }

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task SendEmailConfirmationCode_WhenEmailIsIncorrect_ShouldReturns400Status()
        {
            // Arrange
            var request = new SendConfirmCodeCommand
            {
                Email = "testmail.com"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            if(httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromSeconds(65));
                httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            }

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SendEmailConfirmationCode_WhenSendALotRequests_ShouldReturns429Status()
        {
            // Arrange
            var request = new SendConfirmCodeCommand
            {
                Email = "test@mail.com"
            };

            // Act
            await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }

        [Fact]
        public async Task SendEmailConfirmationCode_WhenEmailIsntRegistered_ShouldReturns200StatusAndSetInCacheEmailAsUnconfirmed()
        {
            // Arrange
            var request = new SendConfirmCodeCommand
            {
                Email = "test@mail.com"
            };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            if (httpResponse.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(TimeSpan.FromSeconds(65));
                httpResponse = await client.PostAsJsonAsync("/api/Auth/SendEmailConfirmationCode", request);
            }

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var cacheData = cache.GetData<UserCode>("unconfirmed_mail:" + request.Email);
            cacheData.Should().NotBeNull();
        }

        [Fact]
        public async Task VerifyConfirmationCode_WhenInvalidRequestData_ShouldReturns400Status()
        {
            // Arrange
            var request = new VerifyCodeCommand
            {
                Code = "12334",
                Email = "testmail.com"
            };
            var userCode = new UserCode
            {
                Code = "12334",
                SendingTime = DateTime.UtcNow,
            };
            await cache.SetData("unconfirmed_mail:" + request.Email, userCode, 30);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/VerifyConfirmationCode", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task VerifyConfirmationCode_WhenInvalidCodeOrEmail_ShouldReturns400Status()
        {
            // Arrange
            var request = new VerifyCodeCommand
            {
                Code = "12334",
                Email = "test@mail.com"
            };
            var userCode = new UserCode
            {
                Code = "124",
                SendingTime = DateTime.UtcNow,
            };
            await cache.SetData("unconfirmed_mail:" + request.Email, userCode, 30);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/VerifyConfirmationCode", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task VerifyConfirmationCode_WhenCodeIsCorrect_ShouldReturns200StatusAndSetCacheData()
        {
            // Arrange
            var request = new VerifyCodeCommand
            {
                Code = "12334",
                Email = "test@mail.com"
            };
            var userCode = new UserCode
            {
                Code = "12334",
                SendingTime = DateTime.UtcNow,
            };
            await cache.SetData("unconfirmed_mail:" + request.Email, userCode, 30);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/Auth/VerifyConfirmationCode", request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var removedCache = await cache.GetData<UserCode>("unconfirmed_mail:" + request.Email);
            removedCache.IsFailure.Should().BeTrue();

            var addedCache = await cache.GetData<string>("confirmed_email:" + request.Email);
            addedCache.Should().NotBeNull();
        }

        [Fact]
        public async Task Logout_ShouldReturns200StatusAndRemoveRefreshToken()
        {
            // Arrange
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + requestToRegister.Email, "Confirmed", 600);
            var registeredResponse = await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            var cookies = registeredResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            var oldRefreshToken = cookies.FirstOrDefault(x => x.StartsWith("refresh_token="));

            // Act
            var httpResponse = await client.PostAsync("/api/Auth/Logout", null);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var userRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == oldRefreshToken);
            userRefreshToken.Should().BeNull();
        }

        [Fact]
        public async Task Refresh_WhenTokenDoesntExist_ShouldReturns401Status()
        {
            // Arrange

            // Act
            var httpResponse = await client.PostAsync("/api/Auth/Refresh", null);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Refresh_WhenTokenExists_ShouldReturns200StatusAndSetNewTokens()
        {
            // Arrange
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + requestToRegister.Email, "Confirmed", 600);
            var registeredResponse = await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            var cookies = registeredResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            var oldRefreshToken = cookies.FirstOrDefault(x => x.StartsWith("refresh_token="));

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/Refresh");
            request.Headers.Add("Cookie", cookies);
            var httpResponse = await client.SendAsync(request);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            cookies = httpResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;

            var newRefreshToken = cookies.FirstOrDefault(x => x.StartsWith("refresh_token="));

            newRefreshToken.Should().NotBeEquivalentTo(oldRefreshToken);
        }

        [Fact]
        public async Task Refresh_WhenTokenIsExpired_ShouldReturns401Status()
        {
            // Arrange
            var requestToRegister = new RegisterCommand
            {
                Email = "test@mail.com",
                Password = "string321",
                UserName = "Test"
            };
            await cache.SetData("confirmed_email:" + requestToRegister.Email, "Confirmed", 600);
            var registeredResponse = await client.PostAsJsonAsync("/api/Auth/Register", requestToRegister);

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Email == requestToRegister.Email);

            var dbRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            // set ExpireDate
            var type = typeof(RefreshToken);
            var property = type.GetProperty("ExpireOn");
            var method = property!.GetSetMethod(true);
            method!.Invoke(dbRefreshToken, [DateTime.UtcNow.AddDays(-30)]);

            await context.SaveChangesAsync();

            // Act
            var httpResponse = await client.PostAsync("/api/Auth/Refresh", null);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
