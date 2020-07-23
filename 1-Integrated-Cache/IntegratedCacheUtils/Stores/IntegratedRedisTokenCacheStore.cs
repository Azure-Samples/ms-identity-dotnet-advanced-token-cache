using IntegratedCacheUtils.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegratedCacheUtils.Stores
{
    public class IntegratedRedisTokenCacheStore : IIntegratedTokenCacheStore
    {
        private string _redisConn;
        private readonly string _redisKey = $"msalAccountActivity";

        public IntegratedRedisTokenCacheStore(string redisConn)
        {
            _redisConn = redisConn;
        }

        public async Task<IEnumerable<MsalAccountActivity>> GetAllAccounts()
        {
            var lazyConnection = GetConnectionMultiplexer();
            List<MsalAccountActivity> accounts = new List<MsalAccountActivity>();

            using (ConnectionMultiplexer redis = lazyConnection.Value)
            {
                IDatabase cache = redis.GetDatabase();
                var all = cache.SetScan(new RedisKey("msalAccountActivity_*"), new RedisValue());
                string serializedAccountActivities = await cache.StringGetAsync("msalAccountActivity");

                if (!String.IsNullOrEmpty(serializedAccountActivities))
                {
                    var acc = JsonConvert.DeserializeObject<MsalAccountActivity>(serializedAccountActivities);
                    accounts.Add(acc);
                    accounts = accounts
                                .Where(x => x.FailedToAcquireToken == false)
                                .ToList();
                }
            }

            return accounts;
        }

        public async Task HandleIntegratedTokenAcquisitionFailure(MsalAccountActivity failedAccountActivity)
        {
            var lazyConnection = GetConnectionMultiplexer();
            using (ConnectionMultiplexer redis = lazyConnection.Value)
            {
                IDatabase cache = redis.GetDatabase();
                await cache.SetRemoveAsync(_redisKey, new RedisValue(JsonConvert.SerializeObject(failedAccountActivity)));
            }
        }

        public async Task UpsertMsalAccountActivity(MsalAccountActivity msalAccountActivity)
        {
            var lazyConnection = GetConnectionMultiplexer();
            using (ConnectionMultiplexer redis = lazyConnection.Value)
            {
                IDatabase cache = redis.GetDatabase();
                await cache.StringSetAsync(_redisKey, new RedisValue(JsonConvert.SerializeObject(msalAccountActivity)));
            } 

        }

        private Lazy<ConnectionMultiplexer> GetConnectionMultiplexer()
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(_redisConn);
            });

            return lazyConnection;
        }
    }
}
