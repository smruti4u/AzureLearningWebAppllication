using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CacheService.Redis
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase database;

        const string connectionString = "rediacachelearn.redis.cache.windows.net:6380,password=Ofm8moDtlHiHhKxJcvM2UXiQPLV+brvt5VVH2brw3pM=,ssl=True,abortConnect=False";
        public CacheService()
        {
            database = getDatabase(connectionString);
        }
        public async Task<T> GetData<T>(string key)
        {
            key = key ?? throw new System.ArgumentNullException(nameof(key));
            var result = await database.StringGetAsync(key).ConfigureAwait(false);

            return (result == RedisValue.Null) ? default(T) : JsonConvert.DeserializeObject<T>(result);
        }

        public async Task SetData<T>(string key, T t, TimeSpan timeSpan)
        {
            var json = JsonConvert.SerializeObject(t);
            await database.StringSetAsync(key, json, timeSpan);
        }

        private IDatabase getDatabase(string connectionString)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });
            return lazyConnection.Value.GetDatabase();

        }
    }
}
