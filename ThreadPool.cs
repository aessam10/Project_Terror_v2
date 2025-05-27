using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;
using System.Threading;

namespace Project_Terror_v2
{
   public class ThreadPool
    {
        public static void Execute(Action action, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe(new Extensions.Threading.LazyDelegate(action, timeOut, priority));
        }
        public static void Execute<T>(Action<T> action, T param, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            GenericThreadPool.Subscribe<T>(new Extensions.Threading.Generic.LazyDelegate<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe(Action action, uint period = 1, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe(new Extensions.Threading.TimerRule(action, period, priority));
        }
        public static IDisposable Subscribe<T>(Action<T> action, T param, uint timeOut = 0, ThreadPriority priority = ThreadPriority.Normal)
        {
            return GenericThreadPool.Subscribe<T>(new Extensions.Threading.Generic.TimerRule<T>(action, timeOut, priority), param);
        }
        public static IDisposable Subscribe<T>(Extensions.Threading.Generic.TimerRule<T> rule, T param, Extensions.Threading.StaticPool pool)
        {
            return pool.Subscribe<T>(rule, param);
        }
        public static IDisposable Subscribe<T>(Extensions.Threading.Generic.TimerRule<T> rule, T param)
        {
            return GenericThreadPool.Subscribe<T>(rule, param);
        }

        public static Extensions.Threading.StaticPool GenericThreadPool;
        public static Extensions.Threading.StaticPool ReceivePool, SendPool, CheckUpConnectionReview;

        public static Extensions.Threading.Generic.TimerRule<ServerSockets.SecuritySocket> ConnectionReceive, ConnectionReview, ConnectionSend;

        public static void CreatePools()
        {
#if TEST
            GenericThreadPool = new Extensions.Threading.StaticPool(1).Run();
            ReceivePool = new Extensions.Threading.StaticPool(1).Run();
            SendPool = new Extensions.Threading.StaticPool(1).Run();
            CheckUpConnectionReview = new Extensions.Threading.StaticPool(1).Run();
#else

            GenericThreadPool = new Extensions.Threading.StaticPool(16).Run();
            ReceivePool = new Extensions.Threading.StaticPool(64).Run();
            SendPool = new Extensions.Threading.StaticPool(32).Run();
            CheckUpConnectionReview = new Extensions.Threading.StaticPool(3).Run();
#endif
            ConnectionReview = new Extensions.Threading.Generic.TimerRule<ServerSockets.SecuritySocket>(connectionReview, 1000, ThreadPriority.Lowest);
            ConnectionReceive = new Extensions.Threading.Generic.TimerRule<ServerSockets.SecuritySocket>(connectionReceive, 1, ThreadPriority.Highest);
            ConnectionSend = new Extensions.Threading.Generic.TimerRule<ServerSockets.SecuritySocket>(connectionSend, 1, ThreadPriority.Highest);

            Game.MsgMonster.PoolProcesor.CreatePools();
            Client.PoolProcesor.CreatePools();
            Role.GameMap.CreatePool();


        }
      
        private static void connectionReview(ServerSockets.SecuritySocket wrapper)
        {
            ServerSockets.SecuritySocket.TryReview(wrapper);
        }
        private static void connectionReceive(ServerSockets.SecuritySocket wrapper)
        {
            ServerSockets.SecuritySocket.TryReceive(wrapper);
        }
        private static void connectionSend(ServerSockets.SecuritySocket wrapper)
        {
          ServerSockets.SecuritySocket.TrySend(wrapper);
        }


        public static void ServerInfo()
        {
            MyConsole.Title = "Players Online : " + Database.Server.GamePoll.Count + " Queue Packets " + ServerSockets.PacketRecycle.Count;
        }
   }
}
