using Application.Shared.Exceptions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Test.Dal.Configurations;
using Test.Dal.Persistences;

namespace Test.Dal
{
    public class AppDbContext
    {
        public const string PROFILES_COLLECTION_NAME = "profiles";
        public const string TESTS_COLLECTION_NAME = "tests";
        public const string QUESTION_COLLECTION_NAME = "questions";
        public const string ANSWER_COLLECTION_NAME = "answers";

        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        // Store last insert index for entities, useing for generate next id entity
        private readonly IMongoCollection<BsonDocument> counters;

        public AppDbContext(
            IConfiguration configuration)
        {
            var mongoConnection = configuration
                .GetConnectionString("MongoDbConnection1") ??
                throw new AppConfigurationException("MongoDb Connection String");
            
            client = new MongoClient(mongoConnection);
            database =  client.GetDatabase("Testing");

            counters = database.GetCollection<BsonDocument>("counters");

            ProfileConfiguration.ApplyProfileConfigurations(database.GetCollection<MongoProfile>(PROFILES_COLLECTION_NAME));
        }

        public IMongoClient Client { get => client; }

        public IMongoCollection<MongoProfile> Profiles 
        { 
            get => database.GetCollection<MongoProfile>(PROFILES_COLLECTION_NAME); 
        }

        public IMongoCollection<MongoTest> Tests 
        { 
            get => database.GetCollection<MongoTest>(TESTS_COLLECTION_NAME); 
        }

        public IMongoCollection<MongoQuestion> Questions
        {
            get => database.GetCollection<MongoQuestion>(QUESTION_COLLECTION_NAME);
        }

        public IMongoCollection<MongoQuestionAnswer> Answers 
        { 
            get => database.GetCollection<MongoQuestionAnswer>(ANSWER_COLLECTION_NAME); 
        }
    
        public long GetNextId(string entityName)
        {
            var filter = Builders<BsonDocument>
                .Filter.Eq("_id", entityName);
            var update = Builders<BsonDocument>.Update.Inc("seq", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true,
            };

            var result = counters.FindOneAndUpdate(filter, update, options);

            return result["seq"].ToInt64();
        }
    }
}
