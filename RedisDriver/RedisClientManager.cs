using RedisDriver.Network;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RedisDriver
{
    public static class RedisClientManager
    {
        public static readonly ConcurrentQueue<RedisClient> clients = new ConcurrentQueue<RedisClient>();
        private static readonly ConcurrentQueue<TaskCompletionSource<RedisClient>> taskCompletionSources = new ConcurrentQueue<TaskCompletionSource<RedisClient>>();
        public static void Init(RedisConfig config)
        {
            int count = Math.Max(config.Connects, 1);
            for(int i = 0; i < count; i++)
            {
                clients.Enqueue(new RedisClient(NetworkHelper.ToIPEndPoint(config.Host, config.Port)));
            }
        }

        public static Task<RedisClient> GetClient()
        {
            if (!clients.TryDequeue(out RedisClient client))
            {
                var tcs = new TaskCompletionSource<RedisClient>();
                taskCompletionSources.Enqueue(tcs);
                return tcs.Task;
            }

            return Task.FromResult(client);
        }

        public static void Recycle(RedisClient client)
        {
            if (taskCompletionSources.TryDequeue(out TaskCompletionSource<RedisClient> tcs))
            {
                tcs.SetResult(client);
            }
            else
            {
                clients.Enqueue(client);
            }
        }
    }
}
