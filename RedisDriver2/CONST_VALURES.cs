namespace RedisDriver
{
    class CONST_VALURES
    {
#region Key
        public const string REDIS_COMMAND_DEL = "DEL";
        public const string REDIS_COMMAND_EXISTS = "EXISTS";                        //by rfzs
        public const string REDIS_COMMAND_EXPIRE = "EXPIRE";
        public const string REDIS_COMMAND_EXPIREAT = "EXPIREAT";                    //by rfzs
        public const string REDIS_COMMAND_KEYS = "KEYS";
        public const string REDIS_COMMAND_PERSIST = "PERSIST";                      //by rfzs
        public const string REDIS_COMMAND_PEXPIRE = "PEXPIRE";
        public const string REDIS_COMMAND_PEXPIREAT = "PEXPIREAT";                  //by rfzs
        public const string REDIS_COMMAND_PTTL = "PTTL";
        public const string REDIS_COMMAND_RANDOMKEY = "RANDOMKEY";                  //by rfzs
        public const string REDIS_COMMAND_RENAME = "RENAME";                        //by rfzs
        public const string REDIS_COMMAND_RENAMENX = "RENAMENX";                    //by rfzs
        public const string REDIS_COMMAND_SORT = "SORT";
        public const string REDIS_COMMAND_TTL = "TTL";
        public const string REDIS_COMMAND_TYPE = "TYPE";                            //by rfzs
#endregion
#region String
        public const string REDIS_COMMAND_APPEND = "APPEND";                        //by rfzs
        public const string REDIS_COMMAND_DECR = "DECR";                            
        public const string REDIS_COMMAND_DECRBY = "DECRBY";                        //by rfzs
        public const string REDIS_COMMAND_GET = "GET";
        public const string REDIS_COMMAND_GETSET = "GETSET";                        //by rfzs
        public const string REDIS_COMMAND_INCR = "INCR";                            //by rfzs
        public const string REDIS_COMMAND_INCRBY = "INCRBY";                        //by rfzs
        public const string REDIS_COMMAND_INCRBYFLOAT = "INCRBYFLOAT";              //by rfzs
        public const string REDIS_COMMAND_MGET = "MGET";
        public const string REDIS_COMMAND_MSET = "MSET";
        public const string REDIS_COMMAND_MSETNX = "MSETNX";                        //by rfzs
        public const string REDIS_COMMAND_PSETEX = "PSETEX";                        //by rfzs
        public const string REDIS_COMMAND_SET = "SET";
        public const string REDIS_COMMAND_SETEX = "SETEX";                          //by rfzs
        public const string REDIS_COMMAND_SETNX = "SETNX";                          //by rfzs
        public const string REDIS_COMMAND_STRLEN = "STRLEN";                        //by rfzs
#endregion
#region Hash
        public const string REDIS_COMMAND_HDEL = "HDEL";
        public const string REDIS_COMMAND_HEXISTS = "HEXISTS";                      //by rfzs
        public const string REDIS_COMMAND_HGET = "HGET";                            //by rfzs
        public const string REDIS_COMMAND_HGETALL = "HGETALL";                      //by rfzs
        public const string REDIS_COMMAND_HINCRBY = "HINCRBY";                      //by rfzs
        public const string REDIS_COMMAND_HINCRBYFLOAT = "HINCRBYFLOAT";            //by rfzs
        public const string REDIS_COMMAND_HKEYS = "HKEYS";                          //by rfzs
        public const string REDIS_COMMAND_HLEN = "HLEN";                            //by rfzs
        public const string REDIS_COMMAND_HMGET = "HMGET";
        public const string REDIS_COMMAND_HMSET = "HMSET";
        public const string REDIS_COMMAND_HSET = "HSET";                            //by rfzs
        public const string REDIS_COMMAND_HSETNX = "HSETNX";                        //by rfzs
        public const string REDIS_COMMAND_HVALS = "HVALS";                          //by rfzs
#endregion
#region List
        public const string REDIS_COMMAND_LINDEX = "LINDEX";
        public const string REDIS_COMMAND_LLEN = "LLEN";
        public const string REDIS_COMMAND_LPOP = "LPOP";
        public const string REDIS_COMMAND_LPUSH = "LPUSH";
        public const string REDIS_COMMAND_LPUSHX = "LPUSHX";                        //by rfzs
        public const string REDIS_COMMAND_LRANGE = "LRANGE";
        public const string REDIS_COMMAND_LREM = "LREM";                            //by rfzs
        public const string REDIS_COMMAND_LSET = "LSET";
        public const string REDIS_COMMAND_LTRIM = "LTRIM";                          //by rfzs
        public const string REDIS_COMMAND_RPOP = "RPOP";
        public const string REDIS_COMMAND_RPOPLPUSH = "RPOPLPUSH";                  //by rfzs
        public const string REDIS_COMMAND_RPUSH = "RPUSH";
        public const string REDIS_COMMAND_RPUSHX = "RPUSHX";                        //by rfzs
#endregion
#region Set
        public const string REDIS_COMMAND_SADD = "SADD";                            //by rfzs
        public const string REDIS_COMMAND_SCARD = "SCARD";                          //by rfzs
        public const string REDIS_COMMAND_SDIFF = "SDIFF";                          //by rfzs
        public const string REDIS_COMMAND_SDIFFSTORE = "SDIFFSTORE";                //by rfzs
        public const string REDIS_COMMAND_SISMEMBER = "SISMEMBER";                  //by rfzs
        public const string REDIS_COMMAND_SMEMBERS = "SMEMBERS";                    //by rfzs
        public const string REDIS_COMMAND_SMOVE = "SMOVE";                          //by rfzs
        public const string REDIS_COMMAND_SPOP = "SPOP";                            //by rfzs
        public const string REDIS_COMMAND_SRANDMEMBER = "SRANDMEMBER";              //by rfzs
        public const string REDIS_COMMAND_SREM = "SREM";                            //by rfzs
        public const string REDIS_COMMAND_SUNION = "SUNION";                        //by rfzs
        public const string REDIS_COMMAND_SUNIONSTORE = "SUNIONSTORE";              //by rfzs
#endregion
#region SortedSet
        public const string REDIS_COMMAND_ZADD = "ZADD";                            //by rfzs
        public const string REDIS_COMMAND_ZCARD = "ZCARD";                          //by rfzs
        public const string REDIS_COMMAND_ZCOUNT = "ZCOUNT";                        //by rfzs
        public const string REDIS_COMMAND_ZINCRBY = "ZINCRBY";                      //by rfzs
        public const string REDIS_COMMAND_ZRANGE = "ZRANGE";                        //by rfzs
        public const string REDIS_COMMAND_ZRANGEBYSCORE = "ZRANGEBYSCORE";          //by rfzs
        public const string REDIS_COMMAND_ZRANK = "ZRANK";                          //by rfzs
        public const string REDIS_COMMAND_ZREM = "ZREM";                            //by rfzs
        public const string REDIS_COMMAND_ZREMRANGEBYRANK = "ZREMRANGEBYRANK";      //by rfzs
        public const string REDIS_COMMAND_ZREMRANGEBYSCORE = "ZREMRANGEBYSCORE";    //by rfzs
        public const string REDIS_COMMAND_ZREVRANGE = "ZREVRANGE";                  //by rfzs
        public const string REDIS_COMMAND_ZREVRANGEBYSCORE = "ZREVRANGEBYSCORE";    //by rfzs
        public const string REDIS_COMMAND_ZREVRANK = "ZREVRANK";                    //by rfzs
        public const string REDIS_COMMAND_ZSCORE = "ZSCORE";                        //by rfzs
#endregion
        public const string REDIS_COMMAND_SELECT = "SELECT";
        public const string REDIS_COMMAND_PING = "PING";
        public const string REDIS_COMMAND_INFO = "INFO";
    }
}