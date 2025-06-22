using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Test.Infrastructure.BackgroundServices;
using Test.Integration.Tests.JwtAuthenticationMock.cs;

namespace Test.Integration.Tests.Extensions
{
    public static class TestIntegrationExtensions
    {
        public static void ConfigureTestEnvironment(
            this IServiceCollection services)
        {
            services.ConfigureTestAuthPolicy();
            services.MockHangFire();
            services.RemoveBackgroundServices();
        }

        private static IServiceCollection ConfigureTestAuthPolicy(
            this IServiceCollection services)
        {
            services.AddTransient<IPolicyEvaluator>(serviceProvider =>
                new TestPolicyEvaluator(ActivatorUtilities
                    .CreateInstance<PolicyEvaluator>(serviceProvider)));

            services
             .AddAuthentication(opts =>
             {
                 opts.DefaultAuthenticateScheme = "Test";
             })
             .AddScheme<JwtBearerOptions, JwtAuthHandler>("Test", opts => { });

            services.AddAuthorization(opts =>
            {
                opts.DefaultPolicy = new AuthorizationPolicyBuilder()
                 .AddAuthenticationSchemes("Test")
                 .RequireAuthenticatedUser()
                 .Build();
            });

            return services;
        }

        private static IServiceCollection MockHangFire(
            this IServiceCollection services)
        {
            var hangfireDescriptors = services
                .Where(x => x.ServiceType.FullName?
                    .Contains("hangfire", StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();

            foreach (var descriptor in hangfireDescriptors)
            {
                services.Remove(descriptor);
            }

            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            services.AddHangfire(x =>
            {

                x.UseSimpleAssemblyNameTypeSerializer()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseInMemoryStorage()
                .UseSerializerSettings(jsonSettings);
            });
            services.AddHangfireServer();

            return services;
        }

        private static IServiceCollection RemoveBackgroundServices(
            this IServiceCollection services)
        {
            services.RemoveService(typeof(ClearInactiveSessionsBackgroundService));
            services.RemoveService(typeof(OutboxBackgroundService));
            services.RemoveService(typeof(CreateRedisOmIndexes));

            return services;
        }
    }
}
