using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgTheKillerOfElite : ITournament
    {
       public const uint RewardConquerPoints = 2000;

       public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public Role.GameMap Map;
        public uint DinamicID, Secounds = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public MsgTheKillerOfElite(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                Map = Database.Server.ServerMaps[700];
                DinamicID = Map.GenerateDynamicID();
#if Arabic
                     MsgSchedules.SendInvitation("KillerElite", "ConquerPoints",  343, 142, 1002, 0, 60);
#else
                MsgSchedules.SendInvitation("KillerElite", "ConquerPoints", 343, 142, 1002, 0, 60);
#endif
           
                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 60;
                Process = ProcesType.Idle;
            }
        }
        public void Revive(Extensions.Time32 Timer, Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                    {
                        ushort x = 0;
                        ushort y = 0;
                        Map.GetRandCoord(ref x, ref y);
                        user.Teleport(x, y, Map.ID, DinamicID);
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                user.Player.KillerPkPoints = 0;
                ushort x = 0;
                ushort y = 0;

                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicID);
                return true;
            }
            return false;
        }

        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1))
                {
#if Arabic
                      MsgSchedules.SendSysMesage("TheKillerOfElite has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                    MsgSchedules.SendSysMesage("KillerElite has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
#if Arabic
                     MsgSchedules.SendSysMesage("[TheKillerOfElite] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
#else
                    MsgSchedules.SendSysMesage("[KillerElite] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
#endif
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;

                if (Now > StartTimer.AddMinutes(10))
                {
#if Arabic
                      MsgSchedules.SendSysMesage("TheKillerOfElite has ended. All Players of TheKillerOfElite has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                    MsgSchedules.SendSysMesage("KillerElite has ended. All Players of TheKillerOfElite has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#endif
                    var array = MapPlayers().OrderByDescending(p => p.Player.KillerPkPoints).ToArray();
                    if (array.Length > 0)
                    {
                        var Winner = array.First();
#if Arabic
  MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  ExtremePk. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#else
                        MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  KillerElite. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#endif
                      
                        Winner.Player.ConquerPoints += RewardConquerPoints;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (Winner.Inventory.HaveSpace(2))
                                Winner.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                            else
                                Winner.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall, 2);
#if Arabic
                                 Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                      
#else
                            Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                      
#endif
                         }

                        int x = 1;
                        foreach (var user in array)
                        {
                            if (x > 1)
                            {
                                user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);
#if Arabic
                                     user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                           
#else
                                user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                           
#endif
                            }
                            x++;
                            user.Teleport(286, 159, 1002);
                        }
                    }
                    Process = ProcesType.Dead;
                }

                if (Now > ScoreStamp)
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.KillerPkPoints).ToArray();

                        foreach (var user in MapPlayers())
                        {
#if Arabic
                              Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.KillerPkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
#else
                            Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("---Your Score:" + user.Player.KillerPkPoints + "---", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                            user.Send(msg.GetArray(stream));
                            msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
#endif

                        }

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
#if Arabic
                              Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.KillerPkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(amsg.GetArray(stream));
#else
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.KillerPkPoints.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(amsg.GetArray(stream));
#endif


                            x++;
                        }

                    }


                    ScoreStamp = Now.AddSeconds(4);
                }


            }
        }

        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }

        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }

        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
    }
}
