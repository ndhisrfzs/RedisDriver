using RedisDriver.Async;
using System;
using System.Collections.Generic;

namespace RedisDriver
{
    public interface IRedis : IDisposable
    {
        ETTask<int> HSet<T>(string key, string field, T value);
        ETTask<bool> HMSet<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs);
        ETTask<List<T>> HGetAll<T>(string key);
        ETTask<List<object>> HGetAll(Type type, string key);
        ETTask<T> HGet<T>(string key, string field);
        ETTask<object> HGet(Type type, string key, string field);
        ETTask<int> Del(string key);
        ETTask<int> HDel(string key, string filed);
    }
}
