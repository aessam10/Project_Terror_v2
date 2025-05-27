using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Project_Terror_v2.Game.MsgFloorItem;
using System.IO;
using Project_Terror_v2.ServerSockets;
using ProtoBuf;
namespace Project_Terror_v2.Game.MsgServer
{
    [ProtoContract]
    public class WalkQuery
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Direction;
        [ProtoMember(2, IsRequired = true)]
        public uint UID;
        [ProtoMember(3, IsRequired = true)]
        public uint Running;
        [ProtoMember(4, IsRequired = true)]
        public uint TimeStamp;
    }
    public static unsafe class MsgMovement
    {
        public const uint Walk = 0, Run = 1, Steed = 9;


        public static sbyte[] DeltaMountX = new sbyte[24] { 0, -2, -2, -2, 0, 2, 2, 2, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1 };
        public static sbyte[] DeltaMountY = new sbyte[24] { 2, 2, 0, -2, -2, -2, 0, 2, 2, 1, -1, -2, -2, -1, 1, 2, 2, 1, -1, -2, -2, -1, 1, 2 };

        
        public static unsafe void GetWalk(this ServerSockets.Packet stream, out WalkQuery pQuery)
        {
          pQuery = new WalkQuery();
          pQuery=  stream.ProtoBufferDeserialize<WalkQuery>(pQuery);
        }

        public static unsafe ServerSockets.Packet MovementCreate(this ServerSockets.Packet stream, WalkQuery pQuery)
        {
            stream.InitWriter();
            pQuery.TimeStamp = (uint)Extensions.Time32.Now.Value;
            stream.ProtoBufferSerialize(pQuery);
        
            stream.Finalize(GamePackets.Movement);

            return stream;
        }

        public static uint Bodyyyy = 0;
        public static uint UIDDDD = 1000000;
        public static int eeffect = 1;
        public static int LastClientStamp = 0;
        [PacketAttribute(GamePackets.Movement)]
        public unsafe static void Movement(Client.GameClient client, ServerSockets.Packet packet)
        {


            /*
           
          client.SendSysMesage("GUI -> " + MsgMessage.TestGui);
             ActionQuery action = new ActionQuery()
             {
                 Type = ActionType.OpenDialog,
                 ObjId = client.Player.UID,
                 dwParam = MsgMessage.TestGui,//MsgServer.DialogCommands.JiangHuSetName,
                 wParam1 = client.Player.X,
                 wParam2 = client.Player.Y
             };
             using (var rec = new ServerSockets.RecycledPacket())
             {
                 var apacket = rec.GetStream();
                 client.Send(apacket.ActionCreate(&action));
             }
             MsgMessage.TestGui++;
            // MyConsole.PrintPacketAdvanced(packet.Memory, packet.Size);
             packet.Seek(4);
            // MyConsole.PrintPacketAdvanced(packet.stream, packet.Size);

            /* var array = Database.DBEffects.Effecte.Values.ToArray();
             var effect = array[eeffect];
             client.Player.SendString(MsgStringPacket.StringID.Effect, true, effect);
             MyConsole.WriteLine(effect + " " + eeffect);
             eeffect++;*/


            //  for (int y = 0; y < 20; y++)
            /*    {
                    for (int x = 0; x < 20; x++)
                    {
                        Game.MsgFloorItem.MsgItemPacket FloorPacket = Game.MsgFloorItem.MsgItemPacket.Create();
                        FloorPacket.m_UID = Game.MsgFloorItem.MsgItem.UIDS.Next;
                        FloorPacket.m_ID = Bodyyyy;
                        FloorPacket.m_X = client.Player.X;
                        FloorPacket.m_Y = client.Player.Y;
                        FloorPacket.m_Color = (byte)x;//4;
                      //  FloorPacket.FlowerType = (byte)x;
                        FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.Effect;
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var apacket = rec.GetStream();
                            client.Send(apacket.ItemPacketCreate(FloorPacket));
                        }
                    }
                }
                Console.WriteLine(Bodyyyy);
          //   Bodyyyy = 1360;
                Bodyyyy++;

                //Bodyyyy = 0;*/

            /*var staticrole = new Role.StaticRole(client.Player.X, client.Player.Y, Bodyyyy.ToString(),  (Bodyyyy));
             staticrole.Map = client.Player.Map;

             client.Map.AddStaticRole(staticrole); using (var rec = new ServerSockets.RecycledPacket())
             {
                 var apacket = rec.GetStream();
                 client.Send(staticrole.GetArray(apacket, false));
             }
             Bodyyyy++;
             //Bodyyyy = 930;
           
        //    Bodyyyy = 910;
            // Bodyyyy++;

             /*Game.MsgFloorItem.MsgItemPacket FloorPacket = Game.MsgFloorItem.MsgItemPacket.Create();
             FloorPacket.m_UID = Game.MsgFloorItem.MsgItem.UIDS.Next;
             FloorPacket.m_ID = Bodyyyy;
             FloorPacket.m_X = client.Player.X;
             FloorPacket.m_Y = client.Player.Y;
             FloorPacket.m_Color = 0;
             FloorPacket.DropType = Game.MsgFloorItem.MsgDropID.Effect;
             using (var rec = new ServerSockets.RecycledPacket())
             {
                 var apacket = rec.GetStream();

                 client.Send(apacket.ItemPacketCreate(FloorPacket));
             }
           //  Bodyyyy = 0;
             Bodyyyy += 1;*/
            /*  Role.SobNpc npc = new Role.SobNpc();
                 npc.X = client.Player.X;
                 npc.Y = client.Player.Y;
                 npc.UID = Bodyyyy;
                 npc.Type = (Role.Flags.NpcType.Talker);
                 npc.Mesh = (Role.SobNpc.StaticMesh)Bodyyyy;
                 npc.Name = "B "+Bodyyyy.ToString()+"";
                // npc.MaxHitPoints = 10000000;
                // npc.HitPoints = 10000000 / 2;
                // npc.Sort = 1;
            using(var rec = new ServerSockets.RecycledPacket())
                client.Send(npc.GetArray(rec.GetStream(), true));
          
                 UIDDDD++;
                 if(Bodyyyy < 11550)
                    Bodyyyy = 11550;
                 Bodyyyy += 10;
                // Bodyyyy = 50000;//36980;
                   //  10; //31220;
            
           
                 /*
               if(UIDDDD > 1000000)
                     UIDDDD = 400000;
                 UIDDDD++;
                 Game.MsgMonster.MonsterRole mob = new MsgMonster.MonsterRole(Database.Server.MonsterFamilies.Values.First(), UIDDDD, "", client.Map);
                 mob.Name = "NemesisTyrant " + Bodyyyy + "";
                 mob.UID = UIDDDD;
                 mob.X = client.Player.X;
                 mob.Y = client.Player.Y;
                 mob.Boss = 1;
                 mob.HitPoints = 20000000;
                 mob.Mesh = (Bodyyyy * 10000000 + 1004);//Bodyyyy;
                 MyConsole.WriteLine("Mesh " +mob.Mesh);
                 using (var rec = new ServerSockets.RecycledPacket())
                     client.Send(mob.GetArray(rec.GetStream(), true));

                // Bodyyyy = 930;
                 Bodyyyy ++;
            /*
                client.SendSysMesage("GUI -> " + MsgMessage.TestGui);
                 MsgServer.MsgDataPacket Data = MsgServer.MsgDataPacket.Create();
                 Data.ID = MsgServer.ActionType.OpenDialog;
                 Data.UID = client.Player.UID;
                 Data.dwParam = MsgMessage.TestGui;
                 Data.wParam1 = client.Player.X;
                 Data.wParam2 = client.Player.Y;
                 client.Send((byte*)&Data);
                 MsgMessage.TestGui++;
           


                 /*if (!client.Player.Alive)
                 {
                     client.Player.X = client.Player.Dead_X;
                     client.Player.Y = client.Player.Dead_Y;

                     Game.MsgServer.MsgAttackPacket AttackPacket = Game.MsgServer.MsgAttackPacket.Create();
                     AttackPacket.X = client.Player.Dead_X;
                     AttackPacket.AtkType = Game.MsgServer.MsgAttackPacket.AttackID.Death;
                     AttackPacket.Y = client.Player.Dead_Y;
                     AttackPacket.OpponentUID = client.Player.UID;
                     client.Player.View.SendView((byte*)&AttackPacket, true);
                     return;
                 }*/

            if (client.Player.Away == 1)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                }
            }
            Role.Flags.ConquerAngle dir;

            WalkQuery walkPacket;

            packet.GetWalk(out walkPacket);
            walkPacket.UID = client.Player.UID;


            client.Player.Action = Role.Flags.ConquerAction.None;

            client.OnAutoAttack = false;
            client.Player.RemoveBuffersMovements(packet);

            client.Player.Protect = Extensions.Time32.Now;


            if (walkPacket.Running == MsgMovement.Steed)
            {

                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 24);

                client.Player.View.SendView(packet.MovementCreate(walkPacket), true);


                int newX = client.Player.X + DeltaMountX[(byte)dir];
                int newY = client.Player.Y + DeltaMountY[(byte)dir];
#if TEST
                MyConsole.WriteLine("Steed walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + newX + " Y " + newY + "");
#endif
                if (client.Map == null)
                {
                    client.Teleport(310, 288, 1002);
                    return;
                }
                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, client.Player.X, client.Player.Y))
                    {
#if Arabic
                          client.SendSysMesage("Illegal jumping over the gates detected.");
#else
                        client.SendSysMesage("Illegal jumping over the gates detected.");
#endif

                        client.Pullback();
                        return;
                    }
                }
                /*if (!client.Map.ValidLocation((ushort)newX, (ushort)newY))
                {
                    client.Teleport(client.Player.Px, client.Player.Py, client.Player.Map, client.Player.DynamicID);
                    return;
                }*/
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, newX, newY);
                client.Player.X = (ushort)newX;
                client.Player.Y = (ushort)newY;

                client.Player.Action = Role.Flags.ConquerAction.None;
                client.Player.View.Role(false, packet.MovementCreate(walkPacket));

                if (client.Vigor >= 2)
                    client.Vigor -= 2;
                else
                    client.Vigor = 0;

                client.Send(packet.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));


            }
            else
            {
                ushort walkX = client.Player.X, walkY = client.Player.Y;
                dir = (Role.Flags.ConquerAngle)(walkPacket.Direction % 8);
                Role.Core.IncXY(dir, ref walkX, ref walkY);


#if TEST
                MyConsole.WriteLine("walk direction -> " + dir.ToString() + " " + (byte)dir + ", X " + walkX + " Y " + walkY + "");
#endif
                if (client.Map == null)
                {
                    client.Teleport(310, 288, 1002);
                    return;
                }
                if (client.Player.Map == 1038)
                {
                    if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidWalk(client.TerainMask, out client.TerainMask, walkX, walkY))
                    {
                        client.SendSysMesage("Illegal jumping over the gates detected.");
                        client.Pullback();
                        return;
                    }
                }
                /*if (!client.Map.ValidLocation((ushort)walkX, (ushort)walkY))
                {
                    client.Teleport(client.Player.Px, client.Player.Py, client.Player.Map, client.Player.DynamicID);
                    return;
                }*/

                if (client.Player.ObjInteraction != null)
                {
                    if (client.Player.ObjInteraction.Player.X == client.Player.X && client.Player.ObjInteraction.Player.Y == client.Player.Y)
                    {

                        InterActionWalk query = new InterActionWalk()
                        {
                            Mode = MsgInterAction.Action.Walk,
                            UID = client.Player.UID,
                            OponentUID = client.Player.ObjInteraction.Player.UID,
                            DirectionOne = (byte)dir
                        };

                        client.Player.View.SendView(packet.InterActionWalk(&query), true);

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                        client.Player.X = walkX;
                        client.Player.Y = walkY;
                        client.Player.Angle = dir;

                        client.Player.View.Role(false, packet.InterActionWalk(&query));

                        client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, walkX, walkY);
                        client.Player.ObjInteraction.Player.X = walkX;
                        client.Player.ObjInteraction.Player.Y = walkY;
                        client.Player.ObjInteraction.Player.Angle = dir;

                        client.Player.ObjInteraction.Player.View.Role();
                        return;
                    }
                }
                client.Player.View.SendView(packet.MovementCreate(walkPacket), true);
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, walkX, walkY);
                client.Player.X = walkX;
                client.Player.Y = walkY;
                client.Player.Angle = dir;

                client.Player.View.Role(false, packet.MovementCreate(walkPacket));
            }
            if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);
            if (MsgTournaments.MsgSchedules.SteedRace.IsOn)
            {
                if (MsgTournaments.MsgSteedRace.MAPID == client.Player.Map)
                    MsgTournaments.MsgSchedules.SteedRace.CheckForRaceItems(client);
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(packet);







        }
    }
}
