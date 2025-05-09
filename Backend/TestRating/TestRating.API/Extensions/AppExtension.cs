using Application.Shared.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Sinks.Network;
using Test.API.Grpc;
using TestRating.API.Configurations;
using TestRating.API.Contracts.Feedback;
using TestRating.API.Contracts.FeedbackReply;
using TestRating.API.Contracts.FeedbackReport;
using TestRating.API.Grpc.Services;
using TestRating.API.Validators.FeedbackValidators;
using TestRating.API.Validators.ReplyValidators;
using TestRating.API.Validators.ReportFeedbackValidators;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;

namespace TestRating.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddLogstashLoging(configuration)
                .AddGrpcProvider(configuration)
                .ConfigureFluentValidation();

            services.AddScoped<ITestExternalService, TestExternalService>();

            return services;
        }

        private static IServiceCollection AddGrpcProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var testingServiceUrl = configuration
                .GetValue<string>("ExternalServices:TestingService") ??
                throw new AppConfigurationException("Testing Service Url");

            services.AddGrpcClient<TestService.TestServiceClient>(options =>
            {
                options.Address = new Uri(testingServiceUrl);
            })
            // allow client connect to server with invalid certificate, only for local development
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                return handler;
            });

            return services;
        }

        private static IServiceCollection ConfigureFluentValidation(
            this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();

            services.AddScoped<IValidator<CreateFeedbackRequest>, CreateFeedbackValidator>();
            services.AddScoped<IValidator<ChangeFeedbackRequest>, ChangeFeedbackValidator>();
            services.AddScoped<IValidator<GetFeedbacksByTestIdQuery>, GetTestFeedbacksValidator>();

            services.AddScoped<IValidator<SendReplyRequest>, SendReplyValidator>();
            services.AddScoped<IValidator<ChangeReplyRequest>, ChangeReplyValidator>();
            services.AddScoped<IValidator<GetFeedbackRepliesQuery>, GetFeedbackRepliesValidator>();

            services.AddScoped<IValidator<SendReportRequest>, SendReportValidator>();

            return services;
        }

        private static IServiceCollection AddLogstashLoging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var logstashConf = configuration
                .GetSection("LogstashSettings")
                .Get<LogstashConf>() ??
                throw new AppConfigurationException("Logstash settings");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.TCPSink(
                    logstashConf.Host,
                    logstashConf.Port,
                    new Serilog.Formatting.Json.JsonFormatter())
                .CreateLogger();

            return services;
        }
    }
}
