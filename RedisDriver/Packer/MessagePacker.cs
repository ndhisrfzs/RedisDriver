using MessagePack;
using System;

namespace RedisDriver.Packer
{
    public class MessagePacker : IPacker
    {
        public byte[] Pack<T>(T obj)
        {
            return MessagePackSerializer.Serialize(obj);
        }

        public object UnPack(ArraySegment<byte> buf, Type type)
        {
            if (buf.Count <= 0)
                return null;
            return MessagePackSerializer.NonGeneric.Deserialize(type, buf);
        }

        public T UnPack<T>(ArraySegment<byte> buf)
        {
            if (buf.Count <= 0)
                return default(T);
            return MessagePackSerializer.Deserialize<T>(buf);
        }
    }
}
