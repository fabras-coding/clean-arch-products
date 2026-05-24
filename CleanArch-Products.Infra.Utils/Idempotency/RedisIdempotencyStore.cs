using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CleanArch_Products.Application.Idempotency;
using StackExchange.Redis;

namespace CleanArch_Products.Infra.Utils.Idempotency
{
    public class RedisIdempotencyStore : IIdempotencyStore
    {

        private readonly IConnectionMultiplexer _redis;
        private const string ProcessingMarker = "__PROCESSING__";

        public RedisIdempotencyStore(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<(int StatusCode, string Body)?> GetResponseAsync(string key)
        {
            
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);

            if(value.IsNullOrEmpty)
                return null; // Key doesn't exist - first time seeing this key

            if(value == ProcessingMarker)
                return null; // Still processing - caller should treat as "in flight"

            var payload = JsonSerializer.Deserialize<IdempotencyPayload>(value.ToString());
            return (payload.StatusCode, payload.Body);

        }

        public async Task SaveResponseAsync(string key, int statusCode, string body, TimeSpan timeToLive)
        {
            
            var db = _redis.GetDatabase();
            var payload = JsonSerializer.Serialize(new IdempotencyPayload(statusCode, body));

            await db.StringSetAsync(
                key,
                payload,
                timeToLive
            );

        }

        /// <summary>
        /// Uses redis SET NX (set-if-not-exists) - atomic, no locking needed.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeToLive"></param>
        /// <returns>Returns true only for the winning request.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> TryReserveAsync(string key, TimeSpan timeToLive)
        {
            
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(
                key,
                ProcessingMarker,
                timeToLive,
                When.NotExists
            );

        }

        public async Task DeleteReservationAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        private record IdempotencyPayload(int StatusCode, string Body);
    }
}