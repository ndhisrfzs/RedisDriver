using RedisDriver.Logger;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RedisDriver.Network
{
    public class TCPClient : IDisposable
    {
        private Socket socket;
        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        private int isSending;
        private bool isConnected;

        private PacketParser parser;

        private IPEndPoint remoteAddress;
        private bool isDisposed;

        private Action<TCPClient, SocketError> errorCallback;
        public event Action<TCPClient, SocketError> ErrorCallback
        {
            add
            {
                errorCallback += value;
            }
            remove
            {
                errorCallback -= value;
            }
        }

        private void OnError(SocketError e)
        {
            if (this.isDisposed)
            {
                return;
            }
            this.errorCallback?.Invoke(this, e);
        }

        private Action<Packet> readCallback;
        public event Action<Packet> ReadCallback
        {
            add
            {
                this.readCallback += value;
            }
            remove
            {
                this.readCallback -= value;
            }
        }

        protected void OnRead(Packet packet)
        {
            this.readCallback.Invoke(packet);
        }

        public TCPClient(IPEndPoint iPEndPoint)
        {
            remoteAddress = iPEndPoint;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            parser = new PacketParser(recvBuffer);
            innArgs.Completed += OnComplete;
            outArgs.Completed += OnComplete;

            isConnected = false;
            isSending = 0;
        }

        public void Start()
        {
            if(!isConnected)
            {
                ConnectAsync(this.remoteAddress);
                return;
            }

            StartRecv();
            StartSend();
        }

        public void Send(Stream stream)
        {
            if(isDisposed)
            {
                throw new Exception("TCPClient is disposed, send message error");
            }

            stream.Seek(0, SeekOrigin.Begin);
            sendBuffer.Write(stream);

            if (Interlocked.CompareExchange(ref isSending, 1, 0) == 0)
            {
                StartSend();
            }
        }

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    {
                        OneThreadSynchronizationContext.Instance.Post(OnConnectComplete, e);
                    }
                    break;
                case SocketAsyncOperation.Receive:
                    {
                        OneThreadSynchronizationContext.Instance.Post(OnRecvComplete, e);
                    }
                    break;
                case SocketAsyncOperation.Send:
                    {
                        OneThreadSynchronizationContext.Instance.Post(OnSendComplete, e);
                    }
                    break;
                case SocketAsyncOperation.Disconnect:
                    {
                        OneThreadSynchronizationContext.Instance.Post(OnDisconnectComplete, e);
                    }
                    break;
            }
        }

        private void ConnectAsync(IPEndPoint iPEndPoint)
        {
            outArgs.RemoteEndPoint = iPEndPoint;
            if(socket.ConnectAsync(outArgs))
            {
                return;
            }

            OnConnectComplete(this.outArgs);
        }

        private void OnConnectComplete(object o)
        {
            if(socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if(e.SocketError != SocketError.Success)
            {
                OnError(e.SocketError);
                return;
            }

            e.RemoteEndPoint = null;
            isConnected = true;

            StartRecv();
            if (Interlocked.CompareExchange(ref isSending, 1, 0) == 0)
            {
                StartSend();
            }
        }

        private void StartRecv()
        {
            int size = recvBuffer.ChunkSize - recvBuffer.LastIndex;
            RecvAsync(recvBuffer.Last, recvBuffer.LastIndex, size);
        }

        private void RecvAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                innArgs.SetBuffer(buffer, offset, count);
            }
            catch(Exception ex)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", ex);
            }

            if (socket.ReceiveAsync(innArgs))
            {
                return;
            }

            OnRecvComplete(innArgs);
        }

        private void OnRecvComplete(object o)
        {
            if (socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if (e.SocketError != SocketError.Success)
            {
                OnError(e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                OnError(e.SocketError);
                return;
            }

            recvBuffer.LastIndex += e.BytesTransferred;
            if (recvBuffer.LastIndex == recvBuffer.ChunkSize)
            {
                recvBuffer.AddLast();
                recvBuffer.LastIndex = 0;
            }
            //Console.WriteLine("Recv:" + e.BytesTransferred);

            while (true)
            {
                if (!parser.Parse())
                {
                    break;
                }

                Packet pack = parser.GetPacket();
                try
                {
                    OnRead(pack);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw ex;
                }
            }

            if (socket == null)
            {
                return;
            }

            StartRecv();
        }

        private void StartSend()
        {
            if(!isConnected)
            {
                isSending = 0;
                return;
            }

            int sendSize = sendBuffer.ChunkSize - sendBuffer.FirstIndex;
            if(sendSize > sendBuffer.Length)
            {
                sendSize = (int)sendBuffer.Length;
            }

            SendAsync(sendBuffer.First, sendBuffer.FirstIndex, sendSize);
        }

        private void SendAsync(byte[] buffer, int offset, int count)
        {
            try
            {
                outArgs.SetBuffer(buffer, offset, count);
                //Console.WriteLine("Send:" + count);
            }
            catch (Exception ex)
            {
                throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", ex);
            }
            if (this.socket.SendAsync(this.outArgs))
            {
                return;
            }
            OnSendComplete(this.outArgs);
        }

        private void OnSendComplete(object o)
        {
            if(socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            if(e.SocketError != SocketError.Success)
            {
                OnError(e.SocketError);
                return;
            }

            sendBuffer.FirstIndex += e.BytesTransferred;
            if(sendBuffer.FirstIndex == sendBuffer.ChunkSize)
            {
                sendBuffer.FirstIndex = 0;
                sendBuffer.RemoveFirst();
            }
            if(sendBuffer.Length <= 0)
            {
                isSending = 0;
                return;
            }

            StartSend();
        }

        private void OnDisconnectComplete(object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            this.OnError(e.SocketError);
        }

        public void Dispose()
        {
            if(isDisposed)
            {
                return;
            }

            isDisposed = true;
            isConnected = false;

            socket.Close();
            innArgs.Dispose();
            outArgs.Dispose();
            parser.Dispose();
            parser = null;
            innArgs = null;
            outArgs = null;
            socket = null;
        }
    }
}
