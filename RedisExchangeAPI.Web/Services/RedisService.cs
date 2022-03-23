using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace RedisExchangeAPI.Web.Services
{
    public class RedisService
    {
        private ConnectionMultiplexer _redis;

        private readonly string _redisHost;
        private readonly string _redisPort;
        // For Database for Redis
        public IDatabase db { get; set; }

        public RedisService(IConfiguration configuration)
        {
            
            _redisHost = configuration["Redis:Host"];
            _redisPort = configuration["Redis:Port"];

        }

        public void Connect()
        {
            // Comminucate with Redis Server
            var configString = $"{_redisHost}:{_redisPort}";

            _redis = ConnectionMultiplexer.Connect(configString);

        }

        // db0 , db1 , db2 , .... db15
        public IDatabase GetDb(int db)
        {
            // Default db0
            return _redis.GetDatabase(db);
        }
       


    }
}
