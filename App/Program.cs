using Common;
using MessagePack;
using MessagePack.Resolvers;
using NLog;
using RedisDriver;
using RedisDriver.Packer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            // 异步方法全部会回掉到主线程
            //SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            // 异步方法全部会回掉到主线程
            LogManager.Configuration.Variables["appType"] = "1";
            LogManager.Configuration.Variables["appId"] = "1";
            LogManager.Configuration.Variables["appTypeFormat"] = $"{"1",-8}";
            LogManager.Configuration.Variables["appIdFormat"] = $"{1:D3}";

            //RoleTest();
            TestMessageSerialize();
            //RedisClientManager.Init();
            Run2();
            //int i = 0;
            //for (i = 0; i < 1; i++)
            //{
            //    Test("test" + i);
            //    Test2("test2" + i);
            //    //Run();
            //}

            while (true)
            {
                Thread.Sleep(1);
                //OneThreadSynchronizationContext.Instance.Update();
            }
            //while(true)
            //{
            //    var c = RedisClientManager.clients;
            //    Thread.Sleep(1000);
            //}
        }

        public static async void RoleTest()
        {
            CompositeResolver.RegisterAndSetAsDefault(
                // Resolve DateTime first
                MessagePack.Resolvers.NativeDateTimeResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var b = MessagePackSerializer.Serialize(new Role() { uid = 1, name = "hlll" });
            var ret1 = await redis.HSet("Role:{1}", "1", new Role() {
                uid = 100008,
                name = "",
                sex = 1,
                head_url = "",
                lv = 1,
                exp = 0, 
                diamond = 887978,
                gold = 8865,
                repertory_lv = 1,
                oil = 101236,
                ammo = 101140,
                steel = 69056,
                al = 69311,
                recovery_time = DateTime.Parse("2018-07-11 09:30:01.740"),
                max_map = 0,

            });
            var r = MessagePackSerializer.NonGeneric.Deserialize(typeof(Role), b);

            //var obj = await redis.HGet<Role>("Role:{1}", "1");
            //var obj = await redis.HGet<Role>("Role:{100008}", "100008");
            //obj.al = 0;
        }

        public static void TestMessageSerialize()
        {
            RedisDriver.Packer.MessagePacker pack = new RedisDriver.Packer.MessagePacker();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var tobj = new Data.TestData()
            {
                name = "hello",
                age = 18
            };
            for (int i = 0; i < 1000000; i++)
            {
                var buf = MessagePackSerializer.Serialize(tobj);

                var obj = MessagePackSerializer.Deserialize<Data.TestData>(buf);
                //var obj = pack.UnPack(new ArraySegment<byte>(buf, 0, (int)buf.Length), typeof(TestData));
            }
            sw.Stop();
            Console.WriteLine("MessagePack:"+sw.Elapsed);
        }

        public static async void Run2()
        {
            await Task.Delay(1000);
            int i = 0;
            for (i = 0; i < 1; i++)
            {
                //Test("test" + i);
                Test2("test2" + i);
                //Run();
            }
        }

        public static async void Run()
        {
            await Task.Delay(1);
            IPacker pack = new JsonPacker();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                pack.Pack(new Data.TestData() { name = "hello", age = 18 });
            }
            sw.Stop();
            Console.WriteLine("Pack Time:" + sw.Elapsed);
        }

        static IRedis redis = new MessageRedis(new RedisConfig() { Host = "127.0.0.1", Port = 6379, Connects = 100 });

        public static async void Test(string key)
        {
            Dictionary<string, Data.TestData> keyValues = new Dictionary<string, Data.TestData>();
            for(int i = 0; i < 10; i++)
            {
                keyValues[i.ToString()] = new Data.TestData() { name = "hello", age = i }; 
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                try
                {
                    var result = await redis.HMSet(key, keyValues);
                    var obj = await redis.HGet<Data.TestData>(key, "5");
                    await redis.Del(key);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            sw.Stop();
            Console.WriteLine("Test Time:" + sw.Elapsed);
        }

        public static async void Test2(string key)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 10000; i++)
            {
                var result = await redis.HSet(key, "hello", new Data.TestData() { name = "hello", age = 18 });
                result = await redis.HDel(key, "hello");
            }
            sw.Stop();

            Console.WriteLine("Test2Over Time:" + sw.Elapsed);
        }
    }

    public abstract class Data
    {
        [MessagePackObject]
        public class TestData : Data
        {
            [Key(0)]
            public string name { get; set; }
            [Key(1)]
            public int age { get; set; }
        }
    }

    [Union(0, typeof(Data.TestData))]
    public interface IData
    {

    }
}
