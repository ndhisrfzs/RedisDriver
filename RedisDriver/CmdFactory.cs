﻿using RedisDriver.Packer;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace RedisDriver
{
    public class CmdFactory
    {
        public IPacker pack;
        private readonly ConcurrentQueue<Cmd> queue = new ConcurrentQueue<Cmd>();

        public CmdFactory(IPacker pack)
        {
            this.pack = pack;
        }

        public Cmd CreateCmd(string method)
        {
            Cmd c = Fetch();
            c.Init();
            c.Add(method);
            return c;
        }
        public Cmd CreateCmd(string method, string key)
        {
            Cmd c = Fetch();
            c.Init();
            c.Add(method);
            c.Add(key);
            return c;
        }
        public Cmd CreateCmd(string method, string key, string field)
        {
            Cmd c = Fetch();
            c.Init();
            c.Add(method);
            c.Add(key);
            c.Add(field);
            return c;
        }

        private Cmd Fetch()
        {
            Cmd c;
            if (!queue.TryDequeue(out c))
            {
                c = new Cmd(this);
            }
            return c;
        }

        public void Recycle(Cmd c)
        {
            queue.Enqueue(c);
        }
    }
}
