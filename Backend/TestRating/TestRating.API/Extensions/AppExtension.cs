using Application.Shared.Exceptions;
using Test.API.Grpc;
using TestRating.API.Grpc.Services;
using TestRating.Application.Common.Interfaces;

namespace TestRating.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddGrpcProvider(configuration);

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
    }
}
