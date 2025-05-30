﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgSkillTournament : ITournament
    {

        public const uint MapID = 1505, RewardConquerPoints = 4000, MaxLifes = 3;

        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public KillerSystem KillSystem;
        public DateTime StartTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public Role.GameMap Map;
        public uint DinamicID, Secounds = 0;

        public MsgSkillTournament(TournamentType _Type)
        {
            Process = ProcesType.Dead;
            Type = _Type;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {

                KillSystem = new KillerSystem();
                Map = Database.Server.ServerMaps[MapID];
                DinamicID = Map.GenerateDynamicID();
#if Arabic
                 MsgSchedules.SendInvitation("SkillTournament", "ConquerPoints", 272, 144, 1002, 0, 60);
#else
                MsgSchedules.SendInvitation("SkillTournament", "ConquerPoints", 272, 144, 1002, 0, 60);
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
                        user.Teleport(272, 144, 1002);
                        user.CreateBoxDialog("You've been eliminated , good luck next time.");
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                ushort x = 0;
                ushort y = 0;

                user.Player.SkillTournamentLifes = MaxLifes;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicID);
                user.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 60, true);
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
                    MsgSchedules.SendSysMesage("SkillTournament has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                    MsgSchedules.SendSysMesage("SkillTournament has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                    foreach (var user in MapPlayers())
                    {
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    }
                   
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
#if Arabic
                    MsgSchedules.SendSysMesage("[SkillTournament] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                    MsgSchedules.SendSysMesage("[SkillTournament] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    InfoTimer = DateTime.Now;
                }
            }
            else if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;

                if (Now > StartTimer.AddSeconds(5))
                {

                    if (MapCount() == 1)
                    {
                        var winner = Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).FirstOrDefault();
                        if (winner != null)
                        {
#if Arabic
  MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  SkillTournament. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#else
                            MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  SkillTournament. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#endif

                            winner.Player.ConquerPoints += (int)RewardConquerPoints;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                if (winner.Inventory.HaveSpace(2))
                                    winner.Inventory.Add(stream, Database.ItemType.PowerExpBall, 2);
                                else
                                    winner.Inventory.AddReturnedItem(stream, Database.ItemType.PowerExpBall, 2);
#if Arabic
                                  Winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                    
#else
                                winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints and 2PowerExpBalls. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

#endif
                                winner.Teleport(272, 144, 1002);
                                Process = ProcesType.Dead;
                            }

                        }
                    }
                    else if (MapCount() == 0)
                    {
                        Process = ProcesType.Dead;
                    }
                }
            }

        }
        public int MapCount()
        {
            return MapPlayers().Length;
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }

    }
}
