using FluentValidation;
using FluentValidation.AspNetCore;
using Test.API.Contracts.Question;
using Test.API.Contracts.Test;
using Test.API.Validators.QuestionValidators;
using Test.API.Validators.TestValidators;

namespace Test.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services)
        {
            services
                .AddGrpcProvider()
                .AddFluentValidation();

            return services;
        }

        private static IServiceCollection AddGrpcProvider(
            this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });

            return services;
        }

        private static IServiceCollection AddFluentValidation(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddScoped<IValidator<CreateTestRequest>, CreateTestValidator>();
            services.AddScoped<IValidator<UpdateTestRequest>, UpdateTestValidator>();
            services.AddScoped<IValidator<CreateQuestionRequest>, CreateQuestionValidator>();
            services.AddScoped<IValidator<UpdateQuestionRequest>, UpdateQuestionValidator>();

            return services;
        }
    }
}
