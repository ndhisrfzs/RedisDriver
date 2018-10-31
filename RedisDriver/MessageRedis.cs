using RedisDriver.Packer;

namespace RedisDriver
{
    public class MessageRedis : Redis
    {
        public MessageRedis(RedisConfig config) 
            : base(new MessagePacker(), config)
        {
        }
    }
}
