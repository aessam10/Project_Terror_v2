using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgSchedules
    {
        public static Extensions.Time32 Stamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.TournamentsStamp);

        public static Dictionary<TournamentType, ITournament> Tournaments = new Dictionary<TournamentType, ITournament>();

        public static ITournament CurrentTournament;

        internal static MsgGuildWar GuildWar;
        internal static MsgSuperGuildWar SuperGuildWar;
        internal static MsgArena Arena;
        internal static MsgTeamArena TeamArena;
        internal static MsgClassPKWar ClassPkWar;
        internal static MsgEliteTournament ElitePkTournament;
        internal static MsgTeamPkTournament TeamPkTournament;
        internal static MsgSkillTeamPkTournament SkillTeamPkTournament;
        internal static MsgCaptureTheFlag CaptureTheFlag;
        internal static MsgClanWar ClanWar;
        internal static MsgQuizShow QuizShow;
        internal static MsgTowerOfMystery TowerOfMystery;
        internal static MsgPkWar PkWar;
        internal static MsgDisCity DisCity;
        internal static MsgPowerArena PowerArena;

        internal static MsgChristmasAnimation ChristmasAnimation;

        internal static MsgSquidwardOctopus SquidwardOctopus;

      
      
    
        internal static MsgSteedRace SteedRace;
      

        internal static void Create()
        {
            Tournaments.Add(TournamentType.None, new MsgNone(TournamentType.None));
            Tournaments.Add(TournamentType.DragonWar, new MsgDragonWar(TournamentType.DragonWar));
            Tournaments.Add(TournamentType.FootBall, new MsgFootball(TournamentType.FootBall));
            Tournaments.Add(TournamentType.BattleField, new MsgBattleField(TournamentType.BattleField));
            Tournaments.Add(TournamentType.DBShower, new MsgDBShower(TournamentType.DBShower));
            Tournaments.Add(TournamentType.TeamDeathMatch, new MsgTeamDeathMatch(TournamentType.TeamDeathMatch));
            Tournaments.Add(TournamentType.LastManStand, new MsgLastManStand(TournamentType.LastManStand));
            Tournaments.Add(TournamentType.ExtremePk, new MsgExtremePk(TournamentType.ExtremePk));
            Tournaments.Add(TournamentType.KillerOfElite, new MsgTheKillerOfElite(TournamentType.KillerOfElite));
            Tournaments.Add(TournamentType.TreasureThief, new MsgTreasureThief(TournamentType.TreasureThief));
            Tournaments.Add(TournamentType.FreezeWar, new MsgFreezeWar(TournamentType.FreezeWar));
            Tournaments.Add(TournamentType.SkillTournament, new MsgSkillTournament(TournamentType.SkillTournament));
            Tournaments.Add(TournamentType.KingOfTheHill, new MsgKingOfTheHill(TournamentType.KingOfTheHill));

          //  Tournaments.Add(TournamentType.QuizShow, new MsgQuizShow(TournamentType.QuizShow));

            ChristmasAnimation = new MsgChristmasAnimation();
            CurrentTournament = Tournaments[TournamentType.None];

            PowerArena = new MsgPowerArena();
            SquidwardOctopus = new MsgSquidwardOctopus();

            SuperGuildWar = new MsgSuperGuildWar();
            GuildWar = new MsgGuildWar();
            Arena = new MsgArena();
            TeamArena = new MsgTeamArena();
            ClassPkWar = new MsgClassPKWar(ProcesType.Dead);
            ElitePkTournament = new MsgEliteTournament();
            CaptureTheFlag = new MsgCaptureTheFlag();

            PowerArena = new MsgPowerArena();
            PkWar = new MsgPkWar();
            DisCity = new MsgDisCity();


            TowerOfMystery = new MsgTowerOfMystery();
           
  
            SteedRace = new MsgSteedRace();
            TeamPkTournament = new MsgTeamPkTournament();
            SkillTeamPkTournament = new MsgSkillTeamPkTournament();
         

            MsgBroadcast.Create();
        }
        internal static void SendInvitation(string Name, string Prize, ushort X, ushort Y, ushort map, ushort DinamicID, int Secounds, Game.MsgServer.MsgStaticMessage.Messages messaj = Game.MsgServer.MsgStaticMessage.Messages.None)
        {
#if Arabic
             string Message = " " + Name + " is about to begin! Will you join it? Prize[" + Prize + "]";
#else
            string Message = " " + Name + " is about to begin! Will you join it? Prize[" + Prize + "]";
#endif

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                var packet = new Game.MsgServer.MsgMessage(Message, MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.Center).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                {
                    if (!client.Player.OnMyOwnServer || client.IsConnectedInterServer())
                        continue;
                    client.Send(packet);
                    client.Player.MessageBox(Message, new Action<Client.GameClient>(user => user.Teleport(X, Y, map, DinamicID)), null, Secounds, messaj);
                }
            }
        }
        internal unsafe static void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red, bool SendScren = false)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
#if Encore
                if (ChatType == MsgServer.MsgMessage.ChatMode.BroadcastMessage)
                    ChatType = MsgServer.MsgMessage.ChatMode.World;
#endif
                var packet = new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream);
                foreach (var client in Database.Server.GamePoll.Values)
                    client.Send(packet);
            }
        }
        internal static void CheckUp(Extensions.Time32 clock)
        {
            

            if (clock > Stamp)
            {
                DateTime Now64 = DateTime.Now;

                if (Program.ServerConfig.IsInterServer)
                {
                    if (Now64.Minute == 55 && Now64.Hour == 19 && Now64.DayOfWeek == DayOfWeek.Thursday)
                    {
                        ElitePkTournament.Start();

                    }
/*AlluringWitch = 40120
DarkCrystalofSky = 40121
DarkCrystalofWind = 40122
ThunderCrystal = 40123
DarkCrystalofFire = 40124
Maniac = 40125*/
                    if (Now64.Minute == 0 && Now64.Second < 3)
                    {
                        var map = Database.Server.ServerMaps[3935];
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            if (!map.ContainMobID(40120))
                                Database.Server.AddMapMonster(stream, map, 40120, 390, 386, 4, 4, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                            if (!map.ContainMobID(40121))
                                Database.Server.AddMapMonster(stream, map, 40121, 399, 380, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);

                            if (!map.ContainMobID(40122))
                                Database.Server.AddMapMonster(stream, map, 40122, 382, 393, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);

                            if (!map.ContainMobID(40123))
                                Database.Server.AddMapMonster(stream, map, 40123, 384, 378, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);

                            if (!map.ContainMobID(40124))
                            {
                                Database.Server.AddMapMonster(stream, map, 40124, 398, 395, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);

                                Database.Server.AddMapMonster(stream, map, 40125, 391, 438, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 393, 432, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 428, 393, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 429, 391, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);

                                Database.Server.AddMapMonster(stream, map, 40125, 384, 333, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 387, 335, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 338, 377, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                                Database.Server.AddMapMonster(stream, map, 40125, 343, 373, 3, 3, 1, 0, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            }
                        }
                    }




                    return;
                }
                if (!Database.Server.FullLoading)
                    return;

                

                if (Arena.Proces == ProcesType.Dead)
                {
                    Arena.Proces = ProcesType.Alive;
                }
            

                /*if (GuildWar != null)
                {
                    if (GuildWar.Proces == ProcesType.Idle)
                    {
                        if (Now64 > GuildWar.StampRound)
                            GuildWar.Began();
                    }
                    if (GuildWar.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > GuildWar.StampShuffleScore)
                        {
                            GuildWar.ShuffleGuildScores();
                        }
                    }
                }*/


                /*if (SuperGuildWar != null)
                {
                    if (SuperGuildWar.Proces == ProcesType.Idle)
                    {
                        if (Now64 > SuperGuildWar.StampRound)
                            SuperGuildWar.Began();
                    }
                    if (SuperGuildWar.Proces != ProcesType.Dead)
                    {
                        if (DateTime.Now > SuperGuildWar.StampShuffleScore)
                        {
                            SuperGuildWar.ShuffleGuildScores();
                        }
                        SuperGuildWar.CheckFlags();
                    }
                
                }*/
        

               
                try
                {
                    SteedRace.work(0);

                    PowerArena.CheckUp();
                    if (CaptureTheFlag.Proces == ProcesType.Alive)
                    {
                        CaptureTheFlag.UpdateMapScore();
                        CaptureTheFlag.CheckUpX2();
                        CaptureTheFlag.SpawnFlags();
                    }
                    if (Now64.Hour == 19 && Now64.Minute == 0 && Now64.DayOfWeek == DayOfWeek.Saturday)
                        TeamPkTournament.Start();
                  
     

                    if ((Now64.Hour == 7 && Now64.Minute == 30 || Now64.Hour == 17 && Now64.Minute == 30))
                        DisCity.Open();
                 
                  
                    CurrentTournament.CheckUp();
                    SquidwardOctopus.CheckUp();


                    if (DateTime.Now.Hour == 21 && DateTime.Now.Minute == 1)
                        ClanWar.Open();
                    ClanWar.CheckUp(Now64);

                   
                    DisCity.CheckUp();

                
                    
                }
                catch (Exception e)
                {
                    MyConsole.SaveException(e);
                }

                switch (Now64.DayOfWeek)
                {
                    case DayOfWeek.Friday://vineri
                        {
                            if (Now64.Hour != 19 && Now64.Hour != 21)
                            {
                                if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                {
                                    if (CurrentTournament.Process == ProcesType.Dead)
                                    {
                                        CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                        CurrentTournament.Open();
                                    }
                                }
                            }
                            //   //fridays 19:55 - 20:00 elite pk
                            if (Now64.Hour == 19 && Now64.Minute == 55)
                            {
                                ElitePkTournament.Start();
                            }
                            break;
                        }
                    case DayOfWeek.Monday://luni
                        {
                            if ((Now64.Hour == 12 || Now64.Minute == 19) && Now64.Minute == 55)
                                PowerArena.Start();
                            if (Now64.Hour != 21)
                            {
                                if (Now64.Hour == 14 && Now64.Minute > 19 || Now64.Hour < 14 || Now64.Hour > 14)
                                {
                                    if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                    {
                                        if (CurrentTournament.Process == ProcesType.Dead)
                                        {
                                            CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                            CurrentTournament.Open();
                                        }
                                    }
                                }
                            }
                            if ((Now64.Hour == 12 || Now64.Hour == 20) && Now64.Minute == 30)
                                SquidwardOctopus.Start();

                            if (Now64.Hour == 14 && Now64.Minute == 0)
                            {
                                ClassPkWar.Start();
                            }
                            else if (Now64.Hour == 15 && Now64.Minute == 0)
                            {

                            }
                            break;
                        }
                    case DayOfWeek.Saturday://sambata
                        {
                           
                            if ((Now64.Hour == 12 || Now64.Hour == 20) && Now64.Minute == 30)
                                SquidwardOctopus.Start();

                            if (Now64.Hour == 8 && Now64.Minute == 0)
                            {
                                PkWar.Open();
                                PkWar.CheckUp();
                            }

                            if (Now64.Hour >= 11)
                            {
                                if (GuildWar.Proces == ProcesType.Dead)
                                {
                                    GuildWar.Start();
                                }
                                if (GuildWar.Proces != ProcesType.Dead)
                                    GuildWar.ShuffleGuildScores();
                            }
                            if (Now64.Hour != 20 && Now64.Hour != 19 && Now64.Hour != 21)
                            {
                                if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                {
                                    if (CurrentTournament.Process == ProcesType.Dead)
                                    {
                                        CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                        CurrentTournament.Open();
                                    }
                                }
                            }
                            if (Now64.Hour == 20)
                            {
                                CaptureTheFlag.Start();
                            }
                            if (Now64.Hour == 21)
                            {
                                CaptureTheFlag.CheckFinish();
                            }
                            break;
                        }
                    case DayOfWeek.Sunday://duminica
                        {
                            if ((Now64.Hour == 12 ) && Now64.Minute == 55)
                                PowerArena.Start();
                            if (Now64.Hour != 19 && Now64.Hour != 21)
                            {
                                if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                {
                                    if (CurrentTournament.Process == ProcesType.Dead)
                                    {
                                        CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                        CurrentTournament.Open();
                                    }
                                }
                            } 
                            if (Now64.Hour < 20)
                            {
                                if (GuildWar.Proces == ProcesType.Dead)
                                    GuildWar.Start();
                                if (GuildWar.Proces == ProcesType.Idle)
                                {
                                    if (Now64 > GuildWar.StampRound)
                                        GuildWar.Began();
                                }
                                if (GuildWar.Proces != ProcesType.Dead)
                                {
                                    if (DateTime.Now > GuildWar.StampShuffleScore)
                                    {
                                        GuildWar.ShuffleGuildScores();
                                    }
                                }
                                if (Now64.Hour == 19)
                                {
                                    if (GuildWar.FlamesQuest.ActiveFlame10 == false)
                                    {
#if Arabic
                                                 SendSysMesage("The Flame Stone 9 is Active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild City.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                               
#else
                                        SendSysMesage("The Flame Stone 9 is Active now. Light up the Flame Stone (62,59) near the Stone Pole in the Guild City.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                               
#endif
                                        GuildWar.FlamesQuest.ActiveFlame10 = true;
                                    }
                                }
                                else if (GuildWar.SendInvitation == false && Now64.Hour == 18)
                                {
#if Arabic
                                     SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
#else
                                    SendInvitation("GuildWar", "ConquerPoints", 200, 254, 1038, 0, 60, MsgServer.MsgStaticMessage.Messages.GuildWar);
#endif
                                   
                                    GuildWar.SendInvitation = true;
                                }
                               
                            }
                            else
                            {
                                if (GuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle)
                                    GuildWar.CompleteEndGuildWar();
                            }
                            break;
                        }
                    case DayOfWeek.Thursday:
                        {
                            if ((Now64.Hour == 12 || Now64.Minute == 19) && Now64.Minute == 55)
                                PowerArena.Start();
                            if (Now64.Hour != 20 && Now64.Hour != 21)
                            {
                                if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                {
                                    if (CurrentTournament.Process == ProcesType.Dead)
                                    {
                                        CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                        CurrentTournament.Open();
                                    }
                                }
                            }
                            break;
                        }
                    case DayOfWeek.Tuesday://marti
                        {
                            if ((Now64.Hour == 12 || Now64.Minute == 19) && Now64.Minute == 55)
                                PowerArena.Start();
                            if (Now64.Minute % 20 == 0 && Now64.Second < 2 && Now64.Hour != 21)
                            {
                                if (CurrentTournament.Process == ProcesType.Dead)
                                {
                                    CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                    CurrentTournament.Open();
                                }
                            }
                            if (SuperGuildWar.Proces == ProcesType.Dead)
                                SuperGuildWar.Start();
                            if (SuperGuildWar.Proces == ProcesType.Idle)
                            {
                                if (Now64 > SuperGuildWar.StampRound)
                                    SuperGuildWar.Began();
                            }
                            if (SuperGuildWar.Proces != ProcesType.Dead)
                            {
                                if (DateTime.Now > SuperGuildWar.StampShuffleScore)
                                {
                                    SuperGuildWar.ShuffleGuildScores();
                                }
                                SuperGuildWar.CheckFlags();
                            }
                            break;
                        }
                    case DayOfWeek.Wednesday://miercuri
                        {
                            if ((Now64.Hour == 12) && Now64.Minute == 55)
                                PowerArena.Start();
                            if (Now64.Hour != 20 && Now64.Hour != 21)
                            {
                                if (Now64.Minute % 20 == 0 && Now64.Second < 2)
                                {
                                    if (CurrentTournament.Process == ProcesType.Dead)
                                    {
                                        CurrentTournament = Tournaments[(TournamentType)Program.GetRandom.Next(1, (byte)TournamentType.Count)];
                                        CurrentTournament.Open();
                                    }
                                }
                            }
                            if (Now64.Hour == 20 && Now64.Minute == 1)
                            {
                                SkillTeamPkTournament.Start();
                            }

                            if (Now64.Hour < 20)
                            {

                                if (SuperGuildWar.Proces == ProcesType.Dead)
                                    SuperGuildWar.Start();
                                if (SuperGuildWar.Proces == ProcesType.Idle)
                                {
                                    if (Now64 > SuperGuildWar.StampRound)
                                        SuperGuildWar.Began();
                                }
                                if (SuperGuildWar.Proces != ProcesType.Dead)
                                {
                                    if (DateTime.Now > SuperGuildWar.StampShuffleScore)
                                    {
                                        SuperGuildWar.ShuffleGuildScores();
                                    }
                                    SuperGuildWar.CheckFlags();
                                }
                            }
                            if (Now64.Hour == 20)
                            {
                                if (SuperGuildWar.Proces == ProcesType.Alive || GuildWar.Proces == ProcesType.Idle)
                                    SuperGuildWar.CompleteEndGuildWar();
                            }

                            break;
                        }
                }
                Stamp.Value = clock.Value + KernelThread.TournamentsStamp;
            }
        }
    }
}
