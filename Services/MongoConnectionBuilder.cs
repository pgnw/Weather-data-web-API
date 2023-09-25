using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_data_web_api.Settings;

namespace Weather_data_web_api.Services
{
    public class MongoConnectionBuilder
    {
        private readonly IOptions<MongoConnection> _options;

        public MongoConnectionBuilder(IOptions<MongoConnection> options)
        {
            _options = options;
        }

        public IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            return client.GetDatabase(_options.Value.DatabaseName);
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            var client = new MongoClient(_options.Value.ConnectionString);
            return client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase(string connString, string databaseName)
        {
            var client = new MongoClient(connString);
            return client.GetDatabase(databaseName);
        }
    }
}
