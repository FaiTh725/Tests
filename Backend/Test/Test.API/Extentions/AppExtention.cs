using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Test.API.Contracts.Question;
using Test.API.Contracts.Test;
using Test.API.Filters;
using Test.API.Validators.QuestionValidators;
using Test.API.Validators.TestValidators;

namespace Test.API.Extentions
{
    public static class AppExtention
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services)
        {
            services
                .AddGrpcProvider()
                .AddFluentValidation();

            services.AddScoped<VerifyProfileFilter>();

            return services;
        }

        public static void ConfigureHangfireDashBoard(
            this WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = [new HangfireAuthorizationFilter()]
            });
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
