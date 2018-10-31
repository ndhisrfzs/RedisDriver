using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedisDriver
{
    public interface IRedis : IDisposable
    {
        Task<int> HSet<T>(string key, string field, T value);
        Task<bool> HMSet<T>(string key, IEnumerable<KeyValuePair<string, T>> keyValuePairs);
        Task<List<T>> HGetAll<T>(string key);
        Task<List<object>> HGetAll(Type type, string key);
        Task<T> HGet<T>(string key, string field);
        Task<object> HGet(Type type, string key, string field);
        Task<int> Del(string key);
        Task<int> HDel(string key, string filed);
    }
}
