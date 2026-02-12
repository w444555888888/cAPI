using Microsoft.Extensions.Options;
using MongoDB.Driver;
using BookingApi.Data; 

namespace BookingApi.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> options)
        {
            var mongoSettings = options.Value;
            _database = new MongoClient(mongoSettings.ConnectionString) // 這裡不會報錯
                        .GetDatabase(mongoSettings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
