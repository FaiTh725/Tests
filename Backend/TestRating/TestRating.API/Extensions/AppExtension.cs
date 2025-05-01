namespace TestRating.API.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureApiServices(
            this IServiceCollection services)
        {
            services
                .AddGrpcProvider();

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
    }
}
