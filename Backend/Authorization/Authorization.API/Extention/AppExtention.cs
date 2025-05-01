using Application.Shared.Exceptions;
using Authorization.API.Grpc.Clients;
using Authorization.API.Validators.UserEntity;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Threading.RateLimiting;
using Test.API.Grpc;
using Test.Contracts.Profile;
using TestRating.Contracts.Profile;

namespace Authorization.API.Extention
{
    public static class AppExtention
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddFlientValidation()
                .AddRateLimits()
                .AddGrpcProvider(configuration);

            services.AddScoped<IExternalService<ProfileRequest, ProfileResponse>, TestingProfileClient>();
            services.AddScoped<IExternalService<CreateProfileRequest, CreateProfileResponse>, FeedbackProfileClient>();

            return services;
        }

        private static IServiceCollection AddFlientValidation(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddScoped<IValidator<RegisterCommand>, RegisterUserValidator>();
            services.AddScoped<IValidator<LoginCommand>, LoginUserValidator>();

            return services;
        }

        private static IServiceCollection AddRateLimits(
            this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
             {
                 options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                 options.AddPolicy("confirm_email", httpContext =>
                     RateLimitPartition.GetFixedWindowLimiter(
                         partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                         factory: partition => new FixedWindowRateLimiterOptions
                         {
                             Window = TimeSpan.FromSeconds(60),
                             PermitLimit = 1
                         })
                 );
             });

            return services;
        }

        private static IServiceCollection AddGrpcProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var testingServiceAdress = configuration
                .GetValue<string>("ExternalService:TestingService") ??
                throw new AppConfigurationException("Grpc Test service address");

            services.AddGrpcClient<ProfileService.ProfileServiceClient>("TestingClient", option =>
            {
                option.Address = new Uri(testingServiceAdress);
            });

            var feedbackServiceAdress = configuration
                .GetValue<string>("ExternalService:FeedbackService") ??
                throw new AppConfigurationException("Grpc Feedback service address");

            Console.WriteLine($"Feedback service - {feedbackServiceAdress}");
            Console.WriteLine($"Testing service - {testingServiceAdress}");

            services.AddGrpcClient<ProfileService.ProfileServiceClient>("FeedbackClient", option =>
            {
                option.Address = new Uri(feedbackServiceAdress);
            });

            return services;
        }
    }
}
