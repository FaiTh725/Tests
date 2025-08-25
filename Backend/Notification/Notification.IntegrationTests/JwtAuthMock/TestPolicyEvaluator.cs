using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Notification.IntegrationTests.JwtAuthMock
{
    public  class TestPolicyEvaluator : IPolicyEvaluator
    {
        private readonly PolicyEvaluator policyEvaluator;

        public TestPolicyEvaluator(
            PolicyEvaluator policyEvaluator)
        {
            this.policyEvaluator = policyEvaluator;
        }

        public Task<AuthenticateResult> AuthenticateAsync(
            AuthorizationPolicy policy, HttpContext context)
        {
            var testPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Test")
                    .Combine(new AuthorizationPolicyBuilder("Test")
                        .RequireAuthenticatedUser()
                        .Build())
                    .Build();

            return policyEvaluator.AuthenticateAsync(testPolicy, context);
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(
            AuthorizationPolicy policy,
            AuthenticateResult authenticationResult,
            HttpContext context,
            object? resource)
        {
            var testPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Test")
                .Combine(new AuthorizationPolicyBuilder("Test")
                    .RequireAuthenticatedUser()
                    .Build())
                .Build();

            return policyEvaluator.AuthorizeAsync(
                testPolicy, authenticationResult,
                context, resource);
        }
    }
}
