using RedisDriver.Logger;
using RedisDriver.Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RedisDriver
{
    public class RedisClient : IDisposable
    {
        private TCPClient client;
        private SocketError error;
        private Action<Packet> action;
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
            var action = this.action;
            this.action = null;
            action?.Invoke(packet);
        }

        public Task<Packet> Send(Cmd cmd)
        {
            var tcs = new TaskCompletionSource<Packet>();
            action = (response) =>
            {
                try
                {
                    if(response.error == 1)
                    {
                        throw new Exception("Time Out");
                    }
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(new Exception($"Socket Error", e));
                }
            };

            client.Send(cmd.GetStream());

            return tcs.Task;
        }
        
        public void Dispose()
        {
            RedisClientManager.Recycle(this);
        }
    }
}
