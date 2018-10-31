using RedisDriver.Packer;

namespace RedisDriver
{
    public class JsonRedis : Redis
    {
        public JsonRedis(RedisConfig config)
            : base(new JsonPacker(), config)
        {
        }
    }
}
