using ServiceStack.Text;
using System;
using System.IO;

namespace RedisDriver.Packer
{
    public class JsonPacker : IPacker
    {
        public byte[] Pack<T>(T obj)
        {
            using (var stream = Util.MemoryStreamManager.GetStream("cmd", 256))
            {
                JsonSerializer.SerializeToStream(obj, stream);
                return Util.FastCloneWithResize(stream.GetBuffer(), (int)stream.Length);
            }
        }

        public object UnPack(ArraySegment<byte> buf, Type type)
        {
            using (var stream = new MemoryStream(buf.Array, buf.Offset, buf.Count))
            {
                return JsonSerializer.DeserializeFromStream(type, stream);
            }
        }

        public T UnPack<T>(ArraySegment<byte> buf)
        {
            using (var stream = new MemoryStream(buf.Array, buf.Offset, buf.Count))
            {
                return JsonSerializer.DeserializeFromStream<T>(stream);
            }
        }
    }
}
