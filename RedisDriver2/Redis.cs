using RedisDriver.Async;
using RedisDriver.Pack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisDriver
{
    public abstract class Redis : IRedis
    {
        private readonly IPack pack;
        private readonly CmdFactory factory;

        public Redis(IPack pack)
        {
            this.pack = pack;
            this.factory = new CmdFactory(pack);
        }

        public async ETTask<int> Del(string key)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_DEL, key))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        var strResult = result.Result.ToString();
                        if (int.TryParse(result.Result.ToString(), out int iResult))
                        {
                            return iResult;
                        }
                        else
                        {
                            throw new Exception(strResult);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async ETTask<int> HDel(string key, string field)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HDEL, key, field))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        var strResult = result.Result.ToString();
                        if (int.TryParse(result.Result.ToString(), out int iResult))
                        {
                            return iResult;
                        }
                        else
                        {
                            throw new Exception(strResult);
                        }
                    }
                }
            }
        }

        public async ETTask<T> HGet<T>(string key, string field)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HGET, key, field))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        if (result.Results != null)
                        {
                            return (T)pack.UnPack(result.Results[0], typeof(T));
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                }
            }
        }

        public async ETTask<object> HGet(Type type, string key, string field)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HGET, key, field))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        if (result.Results != null)
                        {
                            return pack.UnPack(result.Results[0], type);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async ETTask<List<T>> HGetAll<T>(string key)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HGETALL, key))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        if (result.Results != null)
                        {
                            int count = result.Results.Count;
                            List<T> results = new List<T>(count);
                            for (int i = 1; i < count; i += 2)
                            {
                                results.Add((T)pack.UnPack(result.Results[i], typeof(T)));
                            }
                            return results;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async ETTask<List<object>> HGetAll(Type type, string key)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HGETALL, key))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        if (result.Results != null)
                        {
                            int count = result.Results.Count;
                            List<object> results = new List<object>(count);
                            for (int i = 1; i < count; i += 2)
                            {
                                results.Add(pack.UnPack(result.Results[i], type));
                            }
                            return results;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async ETTask<bool> HMSet<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HMSET, key))
            {
                using (var client = await RedisClientManager.GetClient())
                {
                    foreach (var item in keyValuePairs)
                    {
                        cmd.Add(item.Key);
                        cmd.AddValue(item.Value);
                    }
                    using (var result = await client.Send(cmd))
                    {
                        if (result.Result != null)
                        {
                            return string.Equals(result.Result.ToString(), "OK");
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public async ETTask<int> HSet<T>(string key, string field, T value)
        {
            using (var cmd = factory.CreateCmd(CONST_VALURES.REDIS_COMMAND_HSET, key, field))
            {
                cmd.AddValue(value);
                using (var client = await RedisClientManager.GetClient())
                {
                    using (var result = await client.Send(cmd))
                    {
                        var strResult = result.Result.ToString();
                        if (int.TryParse(result.Result.ToString(), out int iResult))
                        {
                            return iResult;
                        }
                        else
                        {
                            throw new Exception(strResult);
                        }
                    }
                }
            }
        }
    }
}
