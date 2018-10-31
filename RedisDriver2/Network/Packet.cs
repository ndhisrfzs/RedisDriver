using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RedisDriver.Network
{
    public class Packet : IDisposable
    {
        public int items { get; set; }
        public int size { get; set; }

        public MemoryStream memoryStream;
        public IList<ArraySegment<byte>> Results { get;set; }
        public object Result { get; set; }
        public Packet(MemoryStream memoryStream)
        {
            this.memoryStream = memoryStream;
        }
        public void Reset(MemoryStream memoryStream)
        {
            items = 0;
            size = 0;
            this.memoryStream = memoryStream;
            Result = null;
            Results?.Clear();
            Results = null;
        }
        public void Dispose()
        {
            memoryStream.Dispose();
            PacketPool.Recycle(this);
        }
    }

    public class PacketParser : IDisposable
    {
        private readonly CircularBuffer buffer;
        private Packet packet = null;
        private bool isOk;

        public PacketParser(CircularBuffer buffer)
        {
            this.buffer = buffer;
        }

        public bool Parse()
        {
            if(isOk)
            {
                return true;
            }
            if(packet == null)
            {
                packet = PacketPool.Fetch();
            }

            bool finish = false;

            int items = packet.items;
            int size = packet.size;

            if (size > 0)
            {
                if (buffer.Length < size + 2)   //判断剩余数据是否足够
                {
                    finish = true;
                }
                else
                {
                    if (items == 0)     //单条回复
                    {
                        var offset = (int)packet.memoryStream.Length;
                        buffer.Read(packet.memoryStream, size + 2);
                        packet.Results = new List<ArraySegment<byte>>(1);
                        packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), offset, size + 2));
                        finish = true;
                        isOk = true;
                    }
                    else
                    {
                        var offset = (int)packet.memoryStream.Length;
                        buffer.Read(packet.memoryStream, size + 2);
                        packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), offset, size + 2));
                        if (items == packet.Results.Count)
                        {
                            finish = true;
                            isOk = true;
                        }
                    }
                    size = packet.size = 0;
                }
            }

            int readCount = 0;
            while (!finish)
            {
                if(buffer.ReadLine(packet.memoryStream, out readCount))
                {
                    var bytes = packet.memoryStream.GetBuffer();
                    int offset = (int)(packet.memoryStream.Length - readCount);
                    var replyType = bytes[packet.memoryStream.Length - readCount]; 
                    switch (replyType)
                    {
                        case (byte)'+':     //状态回复
                        case (byte)'-':     //错误回复
                        case (byte)':':     //整数回复
                            {
                                if (items == 0)     //单条回复
                                {
                                    packet.Result = Encoding.UTF8.GetString(bytes, offset + 1, readCount - 3);
                                    finish = true;
                                    isOk = true;
                                }
                                else    //多条回复
                                {
                                    packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), offset + 1, readCount - 1));
                                    if(items == packet.Results.Count)
                                    {
                                        finish = true;
                                        isOk = true;
                                    }
                                }
                            }
                            break;
                        case (byte)'$':     //批量回复
                            {
                                size = int.Parse(Encoding.UTF8.GetString(bytes, offset + 1, readCount - 3));
                                if (size == -1) //批量回复值不存在
                                {
                                    packet.Results = new List<ArraySegment<byte>>(1);
                                    packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), 0, 0));
                                    finish = true;
                                    isOk = true;
                                }
                                else
                                {
                                    if (buffer.Length < size + 2)   //判断剩余数据是否足够
                                    {
                                        finish = true;
                                        packet.size = size;
                                    }
                                    else
                                    {
                                        if (items == 0)     //单条回复
                                        {
                                            offset = (int)packet.memoryStream.Length;
                                            buffer.Read(packet.memoryStream, size + 2);
                                            packet.Results = new List<ArraySegment<byte>>(1);
                                            packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), offset, size + 2));
                                            finish = true;
                                            isOk = true;
                                        }
                                        else
                                        {
                                            offset = (int)packet.memoryStream.Length;
                                            buffer.Read(packet.memoryStream, size + 2);
                                            packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), offset, size + 2));
                                            if (items == packet.Results.Count)
                                            {
                                                finish = true;
                                                isOk = true;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case (byte)'*':     //多条批量回复
                            {
                                items = int.Parse(Encoding.UTF8.GetString(bytes, offset + 1, readCount - 3));
                                if (items <= 0) //没有回复
                                {
                                    packet.Results = new List<ArraySegment<byte>>(1);
                                    packet.Results.Add(new ArraySegment<byte>(packet.memoryStream.GetBuffer(), 0, 0));
                                    finish = true;
                                    isOk = true;
                                }
                                else
                                {
                                    packet.items = items;
                                    packet.Results = new List<ArraySegment<byte>>(items);
                                }
                            }
                            break;
                        default:
                            {
                                throw new Exception("Packet error");
                            }
                    }
                }
                else
                {
                    finish = true;
                }
            }

            return isOk;
        }

        public Packet GetPacket()
        {
            isOk = false;
            var p = packet;
            packet = null;
            return p;
        }

        public void Dispose()
        {
            packet.Dispose();
        }
    }
}
