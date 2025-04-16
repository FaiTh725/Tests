namespace Test.API.Extentions
{
    public static class AppExtention
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
