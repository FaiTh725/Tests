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
        public const string TEST_SESSIONS_COLLECTION_NAME = "sessions";
        public const string PROFILE_ANSWERS_COLLECTION_NAME = "profile_answers";
        public const string GROUPS_COLLECTION_NAME = "groups";
        public const string TEST_ACCESS_COLLECTION_NAME = "test_accesses";
        public const string OUTBOX_MESSAGE_COLLECTION_NAME = "outbox_messages";

        private readonly IMongoClient client;
        private readonly IMongoDatabase database;
        // Store last inserted index for entities, using for generate next id entity
        private readonly IMongoCollection<BsonDocument> counters;

        public AppDbContext(
            IMongoClient mongoClient,
            IMongoDatabase mongoDatabase)
        {
            client = mongoClient;
            database = mongoDatabase;

            counters = database.GetCollection<BsonDocument>("counters");

            ProfileConfiguration.ApplyProfileConfigurations(database.GetCollection<MongoProfile>(PROFILES_COLLECTION_NAME));
            TestAccessConfiguration.ApplyTestAccessConfigurations(database.GetCollection<MongoTestAccess>(TEST_ACCESS_COLLECTION_NAME));
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

        public IMongoCollection<MongoTestSession> Sessions
        {
            get => database.GetCollection<MongoTestSession>(TEST_SESSIONS_COLLECTION_NAME);
        }

        public IMongoCollection<MongoProfileAnswer> ProfileAnswers
        {
            get => database.GetCollection<MongoProfileAnswer>(PROFILE_ANSWERS_COLLECTION_NAME);
        }

        public IMongoCollection<MongoProfileGroup> Groups
        {
            get => database.GetCollection<MongoProfileGroup>(GROUPS_COLLECTION_NAME);
        }

        public IMongoCollection<MongoTestAccess> Accesses
        {
            get => database.GetCollection<MongoTestAccess>(TEST_ACCESS_COLLECTION_NAME);
        }

        public IMongoCollection<MongoOutboxMessage> OutboxMessages
        {
            get => database.GetCollection<MongoOutboxMessage>(OUTBOX_MESSAGE_COLLECTION_NAME);
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
