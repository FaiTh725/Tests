using Application.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Notification.Domain.Interfaces;
using Notification.Infrastructure.Data;
using Notification.Infrastructure.Data.Implementations;

namespace Notification.Infrastructure
{
    public static class StartUp
    {
        public static IServiceCollection ConfigureInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddMongoProvider(configuration);

            services.AddSingleton<AppDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddMongoProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var mongoConnection = configuration
                .GetConnectionString("MongoConnection") ?? 
                throw new AppConfigurationException("MongoDb Connection String");
            var mongoDatabase = configuration
                .GetValue<string>("MongoDatabase") ??
                throw new AppConfigurationException("Service Database Name");

            var mongoClientSettings = MongoClientSettings
                .FromConnectionString(mongoConnection);

            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, _ => true);

            services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

            services.AddSingleton<IMongoDatabase>(provider => provider
            .GetRequiredService<IMongoClient>()
            .GetDatabase(mongoDatabase));

            return services;
        }
    }
}
