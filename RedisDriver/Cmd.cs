using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace RedisDriver
{
    public class Cmd : IDisposable
    {
        private bool isDisposed = false;
        private CmdFactory factory;

        private MemoryStream stream;
        private MemoryStream result;

        private int count = 0;
        public Cmd(CmdFactory factory)
        {
            this.stream = new MemoryStream(); //Util.MemoryStreamManager.GetStream("cmd", 256);
            this.result = new MemoryStream(); //Util.MemoryStreamManager.GetStream("cmd", 256);
            this.factory = factory;
        }

        public void Init()
        {
            isDisposed = false;
            count = 0;
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            result.Seek(0, SeekOrigin.Begin);
            result.SetLength(0);
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
            var buf = factory.pack.Pack(data);
            byte[] headbuf = Encoding.UTF8.GetBytes("$" + buf.Length);
            stream.Write(headbuf, 0, headbuf.Length);
            stream.Write(Util.Eof, 0, 2);
            stream.Write(buf, 0, buf.Length);
            stream.Write(Util.Eof, 0, 2);
            count++;
        }

        public Stream GetStream()
        {
            byte[] headbuf = Encoding.UTF8.GetBytes("*" + count);
            result.Write(headbuf, 0, headbuf.Length);
            result.Write(Util.Eof, 0, 2);
            stream.WriteTo(result);
            return result;
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
            
            isDisposed = true;
        }
    }
}
