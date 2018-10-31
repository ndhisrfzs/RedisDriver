using RedisDriver.Packer;

namespace RedisDriver
{
    public class BsonRedis : Redis
    {
        public BsonRedis(RedisConfig config) 
            : base(new BsonPacker(), config)
        {
        }
    }
}
