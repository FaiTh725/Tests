using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Test.API.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity?.IsAuthenticated ?? false &&
                httpContext.User.IsInRole("Admin");
        }
    }
}
