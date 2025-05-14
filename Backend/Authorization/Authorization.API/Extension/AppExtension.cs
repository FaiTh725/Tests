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

namespace Authorization.API.Extension
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddFlientValidation()
                .AddRateLimits()
                .AddGrpcProvider(configuration);

            services.AddScoped<IExternalService<ProfileRequest, ProfileResponse>, ProfileClient>();

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
            var serverAdress = configuration
                .GetValue<string>("GrpcServer") ??
                throw new AppConfigurationException("Grpc Test service address");

            services.AddGrpcClient<ProfileService.ProfileServiceClient>(option =>
            {
                option.Address = new Uri(serverAdress);
            });

            return services;
        }
    }
}
