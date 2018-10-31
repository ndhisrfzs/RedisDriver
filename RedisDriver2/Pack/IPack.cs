using System;
using System.IO;

namespace RedisDriver.Pack
{
    public interface IPack
    {
        MemoryStream Pack<T>(T obj);
        object UnPack(ArraySegment<byte> buf, Type type);
    }
}
