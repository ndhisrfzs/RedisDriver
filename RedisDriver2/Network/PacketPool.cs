using System.Collections.Concurrent;

namespace RedisDriver.Network
{
    public class PacketPool
    {
        public static readonly ConcurrentQueue<Packet> packets = new ConcurrentQueue<Packet>();

        public static Packet Fetch()
        {
            Packet p;
            if (packets.TryDequeue(out p))
            {
                p.Reset(Util.MemoryStreamManager.GetStream("message", ushort.MaxValue));
            }
            else
            {
                p = new Packet(Util.MemoryStreamManager.GetStream("message", ushort.MaxValue));
            }
            return p;
        }

        public static void Recycle(Packet packet)
        {
            packets.Enqueue(packet);
        }
    }
}
