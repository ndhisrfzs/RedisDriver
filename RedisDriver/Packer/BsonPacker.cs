using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.IO;

namespace RedisDriver.Packer
{
    public class BsonPacker : IPacker
    {
        public byte[] Pack<T>(T obj)
        {
            using (var stream = Util.MemoryStreamManager.GetStream("cmd", 256))
            {
                using (var bsonWriter = new BsonBinaryWriter(stream))
                {
                    BsonSerializer.Serialize(bsonWriter, obj.GetType(), obj);
                }
                return Util.FastCloneWithResize(stream.GetBuffer(), (int)stream.Length);
            }
        }

        public object UnPack(ArraySegment<byte> buf, Type type)
        {
            using (MemoryStream memoryStream = new MemoryStream(buf.Array, buf.Offset, buf.Count))
            {
                var obj = BsonSerializer.Deserialize(memoryStream, type);
                return obj;
            }
        }

        public T UnPack<T>(ArraySegment<byte> buf)
        {
            using (MemoryStream memoryStream = new MemoryStream(buf.Array, buf.Offset, buf.Count))
            {
                var obj = BsonSerializer.Deserialize<T>(memoryStream);
                return obj;
            }
        }
    }
}
