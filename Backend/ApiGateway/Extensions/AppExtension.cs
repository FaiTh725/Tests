using Application.Shared.Exceptions;

namespace ApiGateway.Extensions
{
    public static class AppExtension
    {
        public static IServiceCollection ConfigureAppServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .ConfigureCors(configuration)
                .ConfigureReverseProxy(configuration);

            return services;
        }

        private static IServiceCollection ConfigureCors(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var corsAllowUrl = configuration
                .GetValue<string>("AllowedUrl") ??
                throw new AppConfigurationException("Allowed client url isnt configured");

            services.AddCors(conf =>
            {
                conf.AddPolicy("client", policy => 
                    policy.WithOrigins(corsAllowUrl)
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod());
            });

            return services;
        }

        private static IServiceCollection ConfigureReverseProxy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddReverseProxy()
                .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                // for local development
                .ConfigureHttpClient((context, handler) =>
                {
                    handler.SslOptions.RemoteCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;
                });

            return services;
        }
    }
}
