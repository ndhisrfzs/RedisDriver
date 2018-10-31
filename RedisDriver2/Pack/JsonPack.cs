using System;
using System.IO;
using System.Text;

namespace RedisDriver.Pack
{
    public class JsonPack : IPack
    {
        public MemoryStream Pack<T>(T obj)
        {
            var stream = Util.MemoryStreamManager.GetStream("cmd", 256);
            ServiceStack.Text.JsonSerializer.SerializeToStream(obj, stream);
            return stream;
        }

        public object UnPack(ArraySegment<byte> buf, Type type)
        {
            using (var stream = new MemoryStream(buf.Array, buf.Offset, buf.Count))
            {
                return ServiceStack.Text.JsonSerializer.DeserializeFromStream(type, stream);
            }
        }
    }
}
