using Microsoft.IO;
using System;
using System.Text;

namespace RedisDriver
{
    public class Util
    {
        public static byte[] Eof = Encoding.UTF8.GetBytes("\r\n");
        public static RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

        public static byte[] FastCloneWithResize(byte[] array, int newSize)
        {
            if (newSize < 0) throw new Exception("newSize");

            byte[] array2 = array;
            if (array2 == null)
            {
                array = new byte[newSize];
                return array;
            }

            byte[] array3 = new byte[newSize];
            Buffer.BlockCopy(array2, 0, array3, 0, (array2.Length > newSize) ? newSize : array2.Length);
            return array3;
        }
    }
}
