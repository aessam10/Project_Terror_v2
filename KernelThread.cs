using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2
{
    public class KernelThread
    {
        public const int TournamentsStamp = 1000,
            ChatItemsStamp = 180000,
            RouletteStamp = 1000,
            TeamArena_CreateMatches = 900,
            TeamArena_VerifyMatches = 980,
            TeamArena_CheckGroups = 960,
            Arena_CreateMatches = 1100,
            Arena_VerifyMatches = 1200,
            Arena_CheckGroups = 1150,
            TeamPkStamp = 1000,
            ElitePkStamp = 1000,
            AccServerStamp = 3300,
            BroadCastStamp = 1000,
            ResetDayStamp = 6000,
            SaveDatabaseStamp = 180000,
            RespawnMapMobs = 500;

        //The Snow Banshee appeared in Frozen Grotto 2(540,430)! Defeat it!

        public Extensions.Time32 UpdateServerStatus = Extensions.Time32.Now;
        public Extensions.ThreadGroup.ThreadItem Thread;
        public KernelThread(int interval, string name)
        {
            Thread = new Extensions.ThreadGroup.ThreadItem(interval, name, OnProcess);
        }
        public void Start()
        {
            Thread.Open();
        }

        public void OnProcess()
        {
            Extensions.Time32 clock = Extensions.Time32.Now;
        //    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
         //   timer.Start();
            try
            {
                if (clock > UpdateServerStatus.AddSeconds(5))
                {
                    if(Program.ServerConfig.IsInterServer)
                        MyConsole.Title = "["+Database.GroupServerList.MyServerInfo.Name+"]QueuePackets: " + ServerSockets.PacketRecycle.Count + " Online " + Database.Server.GamePoll.Count + " Time: " + DateTime.Now.Hour + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Second + "";
                        else
                    MyConsole.Title = "QueuePackets: " + ServerSockets.PacketRecycle.Count + " Online " + Database.Server.GamePoll.Count + " Time: " + DateTime.Now.Hour + "/" + DateTime.Now.Minute + "/" + DateTime.Now.Second + "";
                    UpdateServerStatus = Extensions.Time32.Now.AddSeconds(5);
                }
                if (clock > Program.ResetRandom)
                {

                    Program.GetRandom.SetSeed(Environment.TickCount);

                    Program.ResetRandom = Extensions.Time32.Now.AddMinutes(30);
                }
                Game.MsgTournaments.MsgSchedules.CheckUp(clock);
                Program.GlobalItems.Work(clock);

                foreach (var roullet in Database.Roulettes.RoulettesPoll.Values)
                    roullet.work(clock);


                Game.MsgTournaments.MsgSchedules.TeamArena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.TeamArena.VerifyMatches(clock);

                Game.MsgTournaments.MsgSchedules.Arena.CheckGroups(clock);
                Game.MsgTournaments.MsgSchedules.Arena.CreateMatches(clock);
                Game.MsgTournaments.MsgSchedules.Arena.VerifyMatches(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);
                foreach (var elitegroup in Game.MsgTournaments.MsgSkillTeamPkTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                foreach (var elitegroup in Game.MsgTournaments.MsgEliteTournament.EliteGroups)
                    elitegroup.timerCallback(clock);

                WebServer.Proces.work(clock);
                Game.MsgTournaments.MsgBroadcast.Work(clock);
                Database.Server.Reset(clock);
                Program.SaveDBPayers(clock);

                DateTime DateNow = DateTime.Now;
                if (DateNow.Minute == 15 && DateNow.Second < 1)//re-spawn ganoderma
                {
                    var map = Database.Server.ServerMaps[1011];
                    if (!map.ContainMobID(3130))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 3130, 667, 753, 18, 18, 1);
#if Arabic
                                   Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema & Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                     
#else
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema & Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                     
#endif
                        }
                    }
                }
                if (DateNow.Minute == 16 && DateNow.Second < 1)//re-spawn titan
                {
                    var map = Database.Server.ServerMaps[1020];
                    if (!map.ContainMobID(3134))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 3134, 419, 618, 18, 18, 1);
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The Ganodema & Titan have spawned in the forest/canyon! Hurry to kill them. Drop [Special Items 50% change.].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                        }

                    }
                }

                else if (DateNow.Hour % 5 == 0 && DateNow.Minute == 34 && DateNow.Second <= 1)//re-spawn nemesys
                {
                    var map = Database.Server.ServerMaps[3846];
                    if (!map.ContainMobID(20300))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 20300, 118, 187, 18, 18, 1);
#if Arabic
  Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The NemesisTyrant have spawned in the BloodShedSea on (118, 187) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#else
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The NemesisTyrant have spawned in the BloodShedSea on (118, 187) ! Hurry to kill them. Drop [SavageBone, DragonBalls].", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#endif
                          
                            foreach (var user in Database.Server.GamePoll.Values)
                                user.Player.MessageBox(

#if Arabic
                                    "The NemesisTyrant have spawned in the BloodShedSea on (118, 187) ! Hurry to kill them. Drop [SavageBone, DragonBalls]."
#else
"The NemesisTyrant have spawned in the BloodShedSea on (118, 187) ! Hurry to kill them. Drop [SavageBone, DragonBalls]."
#endif
, new Action<Client.GameClient>(p =>
                                {
                                    p.Teleport(118, 187, 3846);
                                }
                                                      ), null, 60);

                        }
                    }
                }

                if (DateNow.Hour % 3 == 0 && DateNow.Minute == 54 && DateNow.Second < 1)//re-spawn chaos guard
                {
                    var map = Database.Server.ServerMaps[1005];
                    if (!map.ContainMobID(213883))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            Database.Server.AddMapMonster(stream, map, 213883, 50, 50, 18, 18, 1);
                            //The ChaosGuard appeared in Arena (50,50)! Defeat it!
#if Arabic
  Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The ChaosGuard appeared in Arena (50,50)! Defeat it!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#else
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage("The ChaosGuard appeared in Arena (50,50)! Defeat it!", Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));

#endif
                          
                            foreach (var user in Database.Server.GamePoll.Values)
                                user.Player.MessageBox(
#if Arabic
                                    "The ChaosGuard appeared in Arena (50,50)! Defeat it!"
#else
"The ChaosGuard appeared in Arena (50,50)! Defeat it!"
#endif

, new Action<Client.GameClient>(p =>
                                {
                                    p.Teleport(50, 50, 1005);
                                }
                                                      ), null, 60);

                        }
                    }
                }
                if (DateNow.Hour % 3 == 0 && DateNow.Minute == 31 && DateNow.Second < 2)
                {
                    var map = Database.Server.ServerMaps[1927];
                     if (!map.ContainMobID(20070))
                     {

                         using (var rec = new ServerSockets.RecycledPacket())
                         {
                             var stream = rec.GetStream();
                             Database.Server.AddMapMonster(stream, map, 20070, 540, 430, 18, 18, 1);
#if Arabic
                              string Messaj = "The Snow Banshee appeared in Frozen Grotto 2(540,430)! Defeat it!";
#else
                             string Messaj = "The Snow Banshee appeared in Frozen Grotto 2(540,430)! Defeat it!";
#endif
                            //"The SnowBanshee have spawned in the FrozenGroto2 on (378,369) ! Hurry to kill them.";
                             Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(Messaj, Game.MsgServer.MsgMessage.MsgColor.red, Game.MsgServer.MsgMessage.ChatMode.Center).GetArray(stream));
                             foreach (var user in Database.Server.GamePoll.Values)
                                 user.Player.MessageBox(Messaj, new Action<Client.GameClient>(p =>
                                 {
                                     p.Teleport(540, 430, 1927);
                                 }
                                                       ), null, 60);
                         }
                     }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
          //  timer.Stop();
        //    if (timer.ElapsedMilliseconds > 0)
           //     Console.WriteLine(timer.ElapsedMilliseconds);
        }
    }
}
