using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisDriver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class RedisTest
    {
        IRedis redis = new JsonRedis(new RedisConfig() { Host = "127.0.0.1", Port = 6379, Connects = 100 });

        [TestMethod]
        public async Task HSetAndDel()
        {
            var setResult = await redis.HSet("hello", "18", new TestData() { name = "hello", age = 18 });
            Assert.AreEqual(setResult, 1);

            var getResult = await redis.HGet<TestData>("hello", "18");
            Assert.AreEqual(getResult.name, "hello");

            var getResult2 = (TestData)await redis.HGet(typeof(TestData), "hello", "18");
            Assert.AreEqual(getResult2.name, "hello");

            var delResult = await redis.HDel("hello", "18");
            Assert.AreEqual(delResult, 1);
        }

        [TestMethod]
        public async Task HMSetAndDel()
        {
            Dictionary<string, TestData> keyValues = new Dictionary<string, TestData>();
            for (int i = 0; i < 10; i++)
            {
                keyValues[i.ToString()] = new TestData() { name = "hello", age = i };
            }

            var setResult = await redis.HMSet("hello", keyValues);
            Assert.AreEqual(setResult, true);

            var getResult = await redis.HGetAll<TestData>("hello");
            Assert.AreEqual(getResult.Count, 10);

            var getResult2 = (await redis.HGetAll(typeof(TestData), "hello")).ConvertAll(c=>(TestData)c);
            Assert.AreEqual(getResult2.Count, 10);

            var delResult = await redis.Del("hello");
            Assert.AreEqual(delResult, 1);
        }
    }

    //foreach (var item in result.Results)
    //{
    //    var t = ServiceStack.Text.JsonSerializer.DeserializeFromString<TestData>(Encoding.UTF8.GetString(item.Array, item.Offset, item.Count));
    //    if (t != null)
    //    {
    //        Console.WriteLine(t.name);
    //    }
    //}
    public class TestData
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
