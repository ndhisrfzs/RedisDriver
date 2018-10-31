using System;
using System.IO;

namespace RedisDriver.Packer
{
    //size: better<betterfull<json<bson
    //speed:better<betterfull<json<bson
    public interface IPacker
    {
        byte[] Pack<T>(T obj);
        object UnPack(ArraySegment<byte> buf, Type type);
        T UnPack<T>(ArraySegment<byte> buf);
    }
}
