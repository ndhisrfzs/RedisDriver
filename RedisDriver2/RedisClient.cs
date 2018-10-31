using RedisDriver.Async;
using RedisDriver.Logger;
using RedisDriver.Network;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace RedisDriver
{
    public class RedisClient : IDisposable
    {
        private TCPClient client;
        private SocketError error;
        private readonly ConcurrentQueue<Action<Packet>> requestCallback = new ConcurrentQueue<Action<Packet>>();
        public RedisClient(IPEndPoint iPEndPoint)
        {
            client = new TCPClient(iPEndPoint);
            client.ErrorCallback += (c, e) => 
            {
                error = e;
                c.Dispose();
            };
            client.ReadCallback += OnRead;
            client.Start();
        }

        public void OnRead(Packet packet)
        {
            try
            {
                Execute(packet);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        private void Execute(Packet packet)
        {
            if(requestCallback.TryDequeue(out Action<Packet> action))
            {
                action.Invoke(packet);
            }
        }

        public ETTask<Packet> Send(Cmd cmd)
        {
            var tcs = new ETTaskCompletionSource<Packet>();

            requestCallback.Enqueue((response) =>
            {
                try
                {
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Socket Error", e));
                }
            });

            client.Send(cmd.GetStream());

            return tcs.Task;
        }

        public void Dispose()
        {
            RedisClientManager.Recycle(this);
        }
    }
}
