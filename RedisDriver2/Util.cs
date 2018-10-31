using Microsoft.IO;
using System.Text;

namespace RedisDriver
{
    public class Util
    {
        public static byte[] Eof = Encoding.UTF8.GetBytes("\r\n");
        public static RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();
    }
}
