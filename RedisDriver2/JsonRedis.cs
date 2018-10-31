using RedisDriver.Pack;

namespace RedisDriver
{
    public class JsonRedis : Redis
    {
        public JsonRedis()
            : base(new JsonPack())
        {
        }
    }
}
