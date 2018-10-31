using RedisDriver.Async;
using RedisDriver.Network;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RedisDriver
{
    public static class RedisClientManager
    {
        //private static int index = 0;
        //public static readonly List<RedisClient> clientList = new List<RedisClient>();
        //public static ETTask<RedisClient> GetClient()
        //{
        //    return ETTask.FromResult(clientList[Interlocked.Increment(ref index) % clientList.Count]);
        //}

        public static readonly ConcurrentQueue<RedisClient> clients = new ConcurrentQueue<RedisClient>();
        private static readonly ConcurrentQueue<ETTaskCompletionSource<RedisClient>> taskCompletionSources = new ConcurrentQueue<ETTaskCompletionSource<RedisClient>>();
        static RedisClientManager()
        {
            for (int i = 0; i < 100; i++)
            {
                clients.Enqueue(new RedisClient(NetworkHelper.ToIPEndPoint("127.0.0.1", 6379)));
            }
            //for(int i = 0; i < 10; i++)
            //{
            //    clientList.Add(new RedisClient(NetworkHelper.ToIPEndPoint("127.0.0.1", 6379)));
            //}
        }

        public static void Init()
        {

        }

        public static ETTask<RedisClient> GetClient()
        {
            if (!clients.TryDequeue(out RedisClient client))
            {
                ETTaskCompletionSource<RedisClient> tcs = new ETTaskCompletionSource<RedisClient>();
                taskCompletionSources.Enqueue(tcs);
                return tcs.Task;
            }

            return ETTask.FromResult(client);
        }

        public static void Recycle(RedisClient client)
        {
            if (taskCompletionSources.TryDequeue(out ETTaskCompletionSource<RedisClient> tcs))
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
