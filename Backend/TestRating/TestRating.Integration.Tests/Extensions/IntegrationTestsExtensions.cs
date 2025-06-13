using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;
using TestRating.Integration.Tests.JwtAuthenticationMock;

namespace TestRating.Integration.Tests.Extensions
{
    public static class IntegrationTestsExtensions
    {
        public static void ConfigureTestAuthPolicy(
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
        }
    }
}
