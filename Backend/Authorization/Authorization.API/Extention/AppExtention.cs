using Authorization.API.Validators.UserEntity;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Threading.RateLimiting;

namespace Authorization.API.Extention
{
    public static class AppExtention
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services)
        {
            services
                .AddFlientValidation()
                .AddRateLimits();

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
    }
}
