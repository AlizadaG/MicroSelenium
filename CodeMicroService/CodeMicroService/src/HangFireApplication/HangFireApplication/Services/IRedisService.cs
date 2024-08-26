using Newtonsoft.Json;
using StackExchange.Redis;

namespace HangFireApplication.Services
{
    public interface IRedisCache { }
    public interface IRedisService<T> where T : IRedisCache
    {
        IEnumerable<T> Get(string key);
        void Delete(string key);
        void Set(string key, T value);
    }
    public class RedisService<T> : IRedisService<T> where T : IRedisCache
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            this._database = redis.GetDatabase();
            this._redis = redis;
        }

        public void Delete(string key)
        {
            _database.KeyDelete(key);
        }

        public void Set(string key, T value)
        {
            var jsonData = JsonConvert.SerializeObject(value);
            _database.StringSet(key, jsonData);
        }

        public IEnumerable<T> Get(string key)
        {
            var jsonData = _database.StringGet(key);
            if (jsonData.IsNullOrEmpty)
                return default;

            return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonData);
        }
    }
}
