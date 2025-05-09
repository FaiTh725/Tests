using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Test.API.Contracts.Question;
using Test.API.Contracts.Test;
using Test.API.Filters;
using Test.API.Validators.QuestionValidators;
using Test.API.Validators.TestValidators;

namespace Test.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection AddCustomizedSwagger(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "Testing Service API", Version = "v1" });
                o.SchemaFilter<EnumSchemaFilter>();
            });

            return services;
        }

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
