using Application.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Test.Dal.Services;
using Test.Domain.Interfaces;

namespace Test.Dal
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDalServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddMongoDbClientProvider(configuration);

            services.AddSingleton<AppDbContext>();

            services.AddScoped<INoSQLUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddMongoDbClientProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var mongoConnection = configuration
                .GetConnectionString("MongoDbConnection") ??
                throw new AppConfigurationException("MongoDb Connection String");
            var mongoDatabase = configuration
                .GetValue<string>("MongoDbName") ??
                throw new AppConfigurationException("Mongo database name");

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
