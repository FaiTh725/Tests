using Authorization.API.Validators.UserEntity;
using Authorization.Application.Commands.UserEntity.Login;
using Authorization.Application.Commands.UserEntity.Register;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Authorization.API.Extension
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services)
        {
            services
                .AddFlientValidation();

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
    }
}
