using RedisDriver.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RedisDriver.Network
{
    public class CircularBuffer : Stream
    {
        public int ChunkSize = 8192;

        private readonly object writeLock = new object();

        private readonly Queue<byte[]> bufferQueue = new Queue<byte[]>();

        private readonly Queue<byte[]> bufferCache = new Queue<byte[]>();

        public int LastIndex { get; set; }

        public int FirstIndex { get; set; }

        private byte[] lastBuffer;

        public CircularBuffer()
        {
            this.AddLast();
        }

        public CircularBuffer(int chunkSize)
        {
            this.ChunkSize = chunkSize;
            this.AddLast();
        }

        public override long Length
        {
            get
            {
                int c = 0;
                if (this.bufferQueue.Count == 0)
                {
                    c = 0;
                }
                else
                {
                    c = (this.bufferQueue.Count - 1) * ChunkSize + this.LastIndex - this.FirstIndex;
                }
                if (c < 0)
                {
                    Log.Error(string.Format("CircularBuffer count:{0} is < 0: {1}, {2}, {3}", c, this.bufferQueue.Count, this.LastIndex, this.FirstIndex));
                }
                return c;
            }
        }

        public void AddLast()
        {
            byte[] buffer;
            if (this.bufferCache.Count > 0)
            {
                buffer = this.bufferCache.Dequeue();
            }
            else
            {
                buffer = new byte[ChunkSize];
            }
            this.bufferQueue.Enqueue(buffer);
            this.lastBuffer = buffer;
        }

        public void RemoveFirst()
        {
            this.bufferCache.Enqueue(bufferQueue.Dequeue());
        }

        public byte[] First
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.bufferQueue.Peek();
            }
        }

        public byte[] Last
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.lastBuffer;
            }
        }

        /// <summary>
        /// 从CircularBuffer读取到stream流中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task ReadAsync(Stream stream)
        {
            long buffLength = this.Length;
            int sendSize = this.ChunkSize - this.FirstIndex;
            if (sendSize > buffLength)
            {
                sendSize = (int)buffLength;
            }

            await stream.WriteAsync(this.First, this.FirstIndex, sendSize);

            this.FirstIndex += sendSize;
            if (this.FirstIndex == this.ChunkSize)
            {
                this.FirstIndex = 0;
                this.RemoveFirst();
            }
        }

        /// <summary>
        /// 从stream流写到CircularBuffer中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task<int> WriteAsync(Stream stream)
        {
            int size = this.ChunkSize - this.LastIndex;

            int n = await stream.ReadAsync(this.Last, this.LastIndex, size);

            if (n == 0)
            {
                return 0;
            }

            this.LastIndex += n;

            if (this.LastIndex == this.ChunkSize)
            {
                this.AddLast();
                this.LastIndex = 0;
            }

            return n;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset + count)
            {
                throw new Exception($"bufferList length < coutn, buffer length: {buffer.Length} {offset} {count}");
            }

            long length = this.Length;
            if (length < count)
            {
                count = (int)length;
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    Buffer.BlockCopy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    Buffer.BlockCopy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += (ChunkSize - this.FirstIndex);
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }

            return count;
        }

        public bool Read(Stream stream, int count)
        {
            long length = this.Length;
            if (length < count)
            {
                return false;
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    stream.Write(this.First, this.FirstIndex, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += (ChunkSize - this.FirstIndex);
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }

            return true;
        }

        private byte lastByte = 0;
        private int readCount = 0;
        public bool ReadLine(Stream stream, out int count)
        {
            long length = this.Length;
            int index = this.FirstIndex;
            count = 0;
            while (length > 0)
            {
                if(this.First[index] == '\n' && lastByte == '\r')
                {
                    stream.Write(this.First, this.FirstIndex, index + 1 - this.FirstIndex);
                    readCount += (index + 1 - this.FirstIndex);
                    this.FirstIndex = index + 1;
                    this.lastByte = 0;
                    if(this.FirstIndex == ChunkSize)
                    {
                        this.FirstIndex = 0;
                        this.RemoveFirst();
                    }
                    count = readCount;
                    readCount = 0;
                    return true;
                }

                lastByte = this.First[index];
                index++;
                length--;

                if(index == ChunkSize)
                {
                    stream.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex);
                    readCount += (ChunkSize - this.FirstIndex);
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                    index = 0;
                }
            }

            return false;
        }

        public void Write(Stream stream)
        {
            lock (writeLock)
            {
                int count = (int)(stream.Length - stream.Position);

                int alreadyCopyCount = 0;
                while (alreadyCopyCount < count)
                {
                    if (LastIndex == ChunkSize)
                    {
                        //最后数据块已满，新增一个数据块
                        AddLast();
                        LastIndex = 0;
                    }

                    int n = count - alreadyCopyCount;   //计算还未写入数据长度
                    if (ChunkSize - LastIndex > n)
                    {
                        //当前数据块剩余字节数大于所需字节数
                        int readCount = stream.Read(lastBuffer, LastIndex, n);
                        LastIndex += readCount;
                        alreadyCopyCount += readCount;
                    }
                    else
                    {
                        //当前数据库剩余字节数小于等于所需字节数
                        int readCount = stream.Read(lastBuffer, LastIndex, ChunkSize - LastIndex);
                        LastIndex += readCount;
                        alreadyCopyCount += readCount;
                    }
                }
            }
        }

        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(new MemoryStream(buffer, offset, count));
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Position { get; set; }
    }
}
