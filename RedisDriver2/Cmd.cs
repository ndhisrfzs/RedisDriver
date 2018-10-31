using System;
using System.IO;
using System.Text;

namespace RedisDriver
{
    public class Cmd : IDisposable
    {
        private bool isDisposed = false;
        private CmdFactory factory;

        private MemoryStream stream;
        private int count = 0;
        public Cmd(CmdFactory factory)
        {
            this.factory = factory;
        }

        public void Init()
        {
            isDisposed = false;
            count = 0;
            stream = Util.MemoryStreamManager.GetStream("cmd", 256);
        }

        public void Add(string key)
        {
            byte[] buf = Encoding.UTF8.GetBytes(key);
            byte[] headbuf = Encoding.UTF8.GetBytes("$" + buf.Length);
            stream.Write(headbuf, 0, headbuf.Length);
            stream.Write(Util.Eof, 0, 2);
            stream.Write(buf, 0, buf.Length);
            stream.Write(Util.Eof, 0, 2);
            count++;
        }

        public void AddValue<T>(T data)
        {
            using (var objStream = factory.pack.Pack(data))
            {
                byte[] headbuf = Encoding.UTF8.GetBytes("$" + objStream.Length);
                stream.Write(headbuf, 0, headbuf.Length);
                stream.Write(Util.Eof, 0, 2);
                objStream.WriteTo(stream);
                stream.Write(Util.Eof, 0, 2);
                count++;
            }
        }

        public Stream GetStream()
        {
            using (var dataStream = stream)
            {
                stream = Util.MemoryStreamManager.GetStream("cmd", 256);
                byte[] headbuf = Encoding.UTF8.GetBytes("*" + count);
                stream.Write(headbuf, 0, headbuf.Length);
                stream.Write(Util.Eof, 0, 2);
                dataStream.WriteTo(stream);
                return stream;
            }
        }

        ~Cmd()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                //释放托管资源
                count = 0;
                factory.Recycle(this);
            }
            //释放非托管资源
            stream.Dispose();

            isDisposed = true;
        }
    }
}
