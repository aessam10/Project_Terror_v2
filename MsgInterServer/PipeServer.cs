﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project_Terror_v2.Game.MsgServer;
using Project_Terror_v2.MsgInterServer.Packets;

namespace Project_Terror_v2.MsgInterServer
{
    public class PipeServer
    {
        public class User
        {
            public ServerSockets.SecuritySocket Socket;
            public Client.GameClient Owner;
            public bool Alive { get; private set; }
            public User(ServerSockets.SecuritySocket _socket)
            {
                Socket = _socket;
                Alive = true;
            }
        

            public void Send(ServerSockets.Packet msg)
            {
                if (Alive)
                {
                    if (IsServer)
                        Socket.Send(msg);
                    else
                        Owner.Send(msg);
                }
            }
            public void Disconnect()
            {
                if (Alive)
                {
                    Alive = false;
                    Socket.Disconnect();
                }
            }


            //for standard connecxion
            public Database.GroupServerList.Server ServerInfo;
            public bool IsServer
            {
                get { return ServerInfo != null; }
            }

        }
        public static Extensions.SafeDictionary<uint, User> PollServers = new Extensions.SafeDictionary<uint, User>();
                                                                            //3999900405
        private static Extensions.Counter UIDCounter = new Extensions.Counter(3999900001);
        private static Extensions.Counter UIDOnElitePKCounter = new Extensions.Counter(1000000);
        public static ServerSockets.ServerSocket Server;
        public static void Initialize()
        {
            // if (Program.ServerConfig.IsInterServer)
            {
                Server = new ServerSockets.ServerSocket(Accept, Receive, Disconnect);
                Server.Initilize(Program.ServerConfig.Port_SendSize, Program.ServerConfig.Port_ReceiveSize, 1000, 1000);
                Server.Open(Database.GroupServerList.MyServerInfo.IPAddress, Database.GroupServerList.MyServerInfo.Port, Program.ServerConfig.Port_BackLog);
            }
        }

        public static void Send(ServerSockets.Packet stream)
        {
            foreach (var server in PollServers.Values)
                server.Send(stream);
        }

        public static void Accept(ServerSockets.SecuritySocket obj)
        {
            var user = new User(obj);
            obj.Client = user;
            obj.OnInterServer = true;
        }

        public static unsafe void Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            ushort PacketID = stream.ReadUInt16();
           
            var user = obj.Client as User;

           
            try
            {
                switch (PacketID)
                {
                    case PacketTypes.InterServer_RoleInfo:
                        {
                            stream.GetInterServerRoleInfo(user.Owner.Player);
                            break;
                        }
                    case PacketTypes.InterServer_SpecialTitles:
                        {
                            string text;
                            stream.GetInterServerSpecialTitles(out text);

                            Database.ServerDatabase.LoadSpecialTitles(user.Owner, text);
                            break;
                        }
                    case PacketTypes.InterServer_CheckTransfer:
                        {
                            user.Owner = new Client.GameClient(obj, true);
                            user.Owner.PipeServer = user;

                            uint type;
                            uint UID;
                            stream.GetInterServerCheckTransfer(out type, out UID);
                            if (System.IO.File.Exists(Program.ServerConfig.DbLocation + "\\Users\\" + UID.ToString() + ".ini"))
                                type = 1;
                            else
                                type = 2;
                            user.Send(stream.InterServerCheckTransferCreate(type, UID));


                            break;
                        }
                    case PacketTypes.InterServer_Achievement:
                        {
                            string text;
                            stream.GetInterServerAchievement(out text);
                            user.Owner.Achievement = new Database.AchievementCollection();
                            user.Owner.Achievement.Load(text);
                            user.Owner.Player.Achievement = new Game.MsgServer.ClientAchievement(user.Owner.Achievement.Value,  user.Owner.Player.UID);
                            break;
                        }
                    case PacketTypes.InterServer_Chi:
                        {
                            uint ChiPoints = 0;
                            string Dragon;
                            string Phoenix;
                            string Turtle;
                            string Tiger;
                            stream.GetInterServerChi(out ChiPoints, out Dragon, out Phoenix, out Turtle, out Tiger);


                            user.Owner.Player.MyChi = new Role.Instance.Chi(user.Owner.Player.UID);
                            user.Owner.Player.MyChi.Name = user.Owner.Player.Name;
                            user.Owner.Player.MyChi.ChiPoints = (int)ChiPoints;
                            user.Owner.Player.MyChi.Dragon.Load(Dragon, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Phoenix.Load(Phoenix, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Turtle.Load(Turtle, user.Owner.Player.UID, user.Owner.Player.Name);
                            user.Owner.Player.MyChi.Tiger.Load(Tiger, user.Owner.Player.UID, user.Owner.Player.Name);

                            if (user.Owner.Player.MyChi.Dragon.UnLocked)
                            {
                                Role.Instance.Chi.ChiPool.TryAdd(user.Owner.Player.MyChi.UID, user.Owner.Player.MyChi);
                                Program.ChiRanking.Upadte(Program.ChiRanking.Dragon, user.Owner.Player.MyChi.Dragon);
                            }
                            if (user.Owner.Player.MyChi.Phoenix.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Phoenix, user.Owner.Player.MyChi.Phoenix);
                            if (user.Owner.Player.MyChi.Tiger.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Tiger, user.Owner.Player.MyChi.Tiger);
                            if (user.Owner.Player.MyChi.Turtle.UnLocked)
                                Program.ChiRanking.Upadte(Program.ChiRanking.Turtle, user.Owner.Player.MyChi.Turtle);

                            break;
                        }
                    case PacketTypes.InterServer_JiangHu:
                        {
                            string text;
                            stream.GetInterServerJiangHu(out text);

                            Database.DBActions.ReadLine readerline = new Database.DBActions.ReadLine(text, '/');
                            readerline.Read((uint)0);
                            Role.Instance.JiangHu jiang = new Role.Instance.JiangHu(user.Owner.Player.UID); 
                            jiang.Name = readerline.Read("");
                            jiang.CustomizedName = readerline.Read("");
                            jiang.Level = readerline.Read((byte)0);
                            jiang.Talent = readerline.Read((byte)0);
                            jiang.FreeTimeToday = readerline.Read((uint)0);
                            jiang.OnJiangMode = readerline.Read((byte)0) == 1;
                            jiang.FreeCourse = readerline.Read((uint)0);
                            jiang.StartCountDwon = DateTime.Now;
                            jiang.CountDownEnd = DateTime.Now.AddSeconds(readerline.Read((uint)0));
                            jiang.RoundBuyPoints = readerline.Read((uint)0);

                            uint cStage = 0;
                            foreach (var stage in jiang.ArrayStages)
                            {
                                cStage += 1;
                                stage.Activate = readerline.Read((byte)0) == 1;

                                foreach (var star in stage.ArrayStars)
                                {

                                    star.Activate = readerline.Read((byte)0) == 1;
                                    star.UID = readerline.Read((ushort)0);
                                    if (star.Activate)
                                    {
                                        star.Typ = jiang.GetValueType(star.UID);
                                        star.Level = jiang.GetValueLevel(star.UID);
                                        if (cStage == 9)
                                        {
                                            if (star.Typ == Role.Instance.JiangHu.Stage.AtributesType.PAttack)
                                            {
                                                star.Typ = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
                                                star.UID = jiang.ValueToRoll(star.Typ, star.Level);
                                            }
                                        }
                                    }
                                }
                            }
                            jiang.CreateStatusAtributes(null);
                            break;
                        }
                    case PacketTypes.InterServer_InnerPower:
                        {
                          

                            string text;
                            stream.GetInterServerInnerPower(out text);
                            Database.DBActions.ReadLine reader = new Database.DBActions.ReadLine(text, '/');
                            Role.Instance.InnerPower item = new Role.Instance.InnerPower(reader.Read(""), user.Owner.Player.UID);
                            reader.Read((uint)0);//uid!
                            item.Potency = reader.Read((int)0);
                            int Stages = reader.Read((int)0);
                            for (int i = 0; i < Stages; i++)
                            {
                                var Stage = item.Stages[i];
                                Stage.ID = reader.Read((ushort)0);
                                Stage.UnLocked = reader.Read((byte)0) == 1;
                                int count_neigongs = reader.Read((int)0);
                                for (int y = 0; y < count_neigongs; y++)
                                {
                                    var neigon = Stage.NeiGongs[y];
                                    neigon.ID = reader.Read((byte)0);
                                    neigon.Score = reader.Read((byte)0);
                                    neigon.Unlocked = reader.Read((byte)0) == 1;
                                    neigon.level = reader.Read((byte)0);
                                    neigon.Complete = reader.Read((byte)0) == 1;
                                }
                            }
                            if (Role.Instance.InnerPower.InnerPowerPolle.ContainsKey(item.UID))
                                Role.Instance.InnerPower.InnerPowerPolle[item.UID] = item;
                            else
                                Role.Instance.InnerPower.InnerPowerPolle.TryAdd(item.UID, item);
                            Role.Instance.InnerPower.InnerPowerRank.UpdateRank(item);
                            break;
                        }
                    case PacketTypes.InterServer_EliteRank:
                        {
                            
                                if (user.IsServer)
                                {
                                    stream.Seek(stream.Size - 8);
                                    stream.Finalize(PacketTypes.InterServer_EliteRank);
                                    foreach (var server in PollServers.Values)
                                        server.Send(stream);
                                }

                          
                            break;
                        }

                    case Game.GamePackets.Chat:
                        {
                           
                            if (Program.ServerConfig.IsInterServer)
                            {
                                var msg = new MsgMessage();
                                msg.Deserialize(stream);
                           
                                if (msg.ChatType == MsgMessage.ChatMode.CrosTheServer)
                                {
                                    stream.Seek(stream.Size -8);
                                    stream.Finalize(1004);
                                    if (user.IsServer)
                                    {

                                        foreach (var server in PollServers.Values)
                                            server.Send(stream);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                                            if (Program.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                                            {
                                                hinvoker(user.Owner, stream);
                                            }
                                            else
                                            {
#if TEST
                                MyConsole.WriteLine("Not found the packet ----> " + PacketID);
#endif
                                            }
                                        }
                                        catch (Exception e) { Console.WriteLine(e.ToString()); }
                                        
                                    }
                                }
                                else
                                {
                                    stream.Seek(stream.Size - 8);
                                    stream.Finalize(1004);
                                    goto default;
                                }
                            }
                            else
                                goto default;
                          
                            break;
                        }
                    case Game.GamePackets.Update:
                        {
                            
                                MsgUpdate.DataType Action;
                                ulong Value;
                                stream.GetUpdatePacket(out Action, out Value);

                                switch (Action)
                                {
                                    case MsgUpdate.DataType.ConquerPoints: user.Owner.Player.ConquerPoints = (uint)Value; break;
                                    case MsgUpdate.DataType.BoundConquerPoints: user.Owner.Player.BoundConquerPoints = (int)Value; break;
                                }
                          
                            break;
                        }
                    case PacketTypes.InterServer_UnionInfo:
                        {
                          
                                uint UID;
                                Role.Instance.Union.Member.MilitaryRanks rank;
                                string Name;
                                string LeaderName;
                                byte IsKingDom;
                                stream.GetUnionInfo(out UID, out rank, out Name, out LeaderName, out IsKingDom);
                                if (string.IsNullOrEmpty(Name) == false && string.IsNullOrEmpty(LeaderName) == false)
                                {
                                    Instance.Union.AddToUnion(stream, user.Owner, UID, rank, Name, LeaderName, IsKingDom);
                                }
                           
                            break;
                        }
                    case PacketTypes.InterServer_GuildInfo:
                        {
                           
                                uint UID;
                                Role.Flags.GuildMemberRank rank;
                                string GuildName;
                                string LeaderName;
                                stream.GetGuildInfo(out UID, out rank, out GuildName, out LeaderName);
                                if (string.IsNullOrEmpty(GuildName) == false && string.IsNullOrEmpty(LeaderName) == false)
                                    Instance.Guilds.AddToGuild(stream, user.Owner, UID, rank, GuildName, LeaderName);
                            
                            break;
                        }
                    case PacketTypes.InterServer_ServerInfo://server info.
                        {
                           
                                uint type;
                                uint ServerID; string ServerName; uint MapID; uint X; uint Y; uint Group;
                                stream.GetServerInfo(out type, out ServerID, out ServerName, out MapID, out X, out Y, out Group);
                                if (type == 1 && ServerID < 10)
                                {
                                    user.ServerInfo = new Database.GroupServerList.Server()
                                    {
                                        ID = ServerID,
                                        Group = Group,
                                        MapID = MapID,
                                        Name = ServerName,
                                        X = X,
                                        Y = Y
                                    };

                                    if (!PollServers.ContainsKey(user.ServerInfo.ID))
                                        PollServers.Add(user.ServerInfo.ID, user);
                                    else
                                        PollServers[user.ServerInfo.ID] = user;
                                }
                        
                            break;
                        }
                    case PacketTypes.InterServer_UnionRanks://union Info.
                        {
                          
                                if (user.IsServer)
                                {
                                    Game.MsgServer.MsgLeagueRank.ActionID type; ushort count;
                                    ushort Page; byte dwparam; ushort PageCount;
                                    stream.GetLeagueRank(out type, out count, out Page, out dwparam, out PageCount);
                                    if (count == 1)
                                    {
                                        uint ServerID; uint GoldBricks; string Name; string LeaderName;
                                        stream.GetItemLeagueRank(out ServerID, out GoldBricks, out Name, out LeaderName);
                                        Instance.Union.AddUnion(ServerID, GoldBricks, Name, LeaderName);


                                        stream.LeagueRankCreate(MsgLeagueRank.ActionID.ShowAllUnions, (ushort)1, (ushort)0, (byte)0, (ushort)0);
                                        stream.AddItemLeagueRank(Database.GroupServerList.MyServerInfo.ID, GoldBricks, Name, LeaderName);

                                        stream.InterServerLeagueRankFinalize();
                                        foreach (var server in PollServers.Values)
                                            server.Send(stream);
                                    }
                                }
                           
                            break;
                        }

                    case Game.GamePackets.HeroInfo:
                        {
                            Role.Player player;

                            if (user.Owner == null)
                            {
                                user.Owner = new Client.GameClient(obj, true);
                                user.Owner.PipeServer = user;
                            }
                            user.Owner.Equipment = new Role.Instance.Equip(user.Owner);
                            stream.GetHeroInfo(user.Owner, out player);

                            if (player.InitTransfer == 1)
                            {
                                player.UID = player.RealUID;

                                user.Owner.Player = player;
                                player.Owner.Map = Database.Server.ServerMaps[1002];
                                player.Owner.Map.View.EnterMap<Role.IMapObj>(player);
                                player.Map = 1002;
                                player.X = 241;
                                player.Y = 242;

                                player.SubClass = new Role.Instance.SubClass();
                                player.MyChi = new Role.Instance.Chi(player.UID);
                                player.InnerPower = new Role.Instance.InnerPower(player.Name, player.UID);
                                user.Owner.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                                user.Owner.ArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.ArenaStatistic.Info.ArenaPoints = 4000;
                                user.Owner.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                                user.Owner.TeamArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.TeamArenaStatistic.Info.ArenaPoints = 4000;

                                if (user.Owner.Player.Flowers == null)
                                {
                                    user.Owner.Player.Flowers = new Role.Instance.Flowers(user.Owner.Player.UID, user.Owner.Player.Name);
                                    user.Owner.Player.Flowers.FreeFlowers = 1;
                                }
                                if (user.Owner.Player.Nobility == null)
                                    user.Owner.Player.Nobility = new Role.Instance.Nobility(user.Owner);
                                if (user.Owner.Player.Associate == null)
                                {
                                    user.Owner.Player.Associate = new Role.Instance.Associate.MyAsociats(user.Owner.Player.UID);
                                    user.Owner.Player.Associate.MyClient = user.Owner;
                                    user.Owner.Player.Associate.Online = true;
                                }
                                player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);
                                
                                player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
                                player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
                                player.Owner.MySpells = new Role.Instance.Spell(player.Owner);

                                if (player.Owner.Achievement == null)
                                    player.Owner.Achievement = new Database.AchievementCollection();
                                Database.Server.GamePoll.TryAdd(user.Owner.Player.UID, user.Owner);


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    obj.Send(msg.MsgInterServerIdentifier(0, player.RealUID, 0));
                                }

                                user.Owner.Player.View.Role();
                                user.Owner.FullLoading = true;
                    
                            }
                            else
                            {
                               // user.Owner = new Client.GameClient(obj, true);
                               // user.Owner.PipeServer = user;

                                uint New_UID = UIDCounter.Next;

                                player.UID = New_UID;

                                user.Owner.Player = player;
                                player.Owner.Map = Database.Server.ServerMaps[1002];
                                player.Owner.Map.View.EnterMap<Role.IMapObj>(player);
                                player.Map = 1002;
                                player.X = 152;
                                player.Y = 219;


                                player.SubClass = new Role.Instance.SubClass();
                                player.MyChi = new Role.Instance.Chi(player.UID);
                                player.InnerPower = new Role.Instance.InnerPower(player.Name, player.UID);
                                user.Owner.ArenaStatistic = new Game.MsgTournaments.MsgArena.User();
                                user.Owner.ArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.ArenaStatistic.Info.ArenaPoints = 4000;
                                user.Owner.TeamArenaStatistic = new Game.MsgTournaments.MsgTeamArena.User();
                                user.Owner.TeamArenaStatistic.ApplayInfo(user.Owner.Player);
                                user.Owner.TeamArenaStatistic.Info.ArenaPoints = 4000;

                                if (user.Owner.Player.Flowers == null)
                                {
                                    user.Owner.Player.Flowers = new Role.Instance.Flowers(user.Owner.Player.UID, user.Owner.Player.Name);
                                    user.Owner.Player.Flowers.FreeFlowers = 1;
                                }
                                if (user.Owner.Player.Nobility == null)
                                    user.Owner.Player.Nobility = new Role.Instance.Nobility(user.Owner);
                                if (user.Owner.Player.Associate == null)
                                {
                                    user.Owner.Player.Associate = new Role.Instance.Associate.MyAsociats(user.Owner.Player.UID);
                                    user.Owner.Player.Associate.MyClient = user.Owner;
                                    user.Owner.Player.Associate.Online = true;
                                }
                                player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);
                                player.Owner.Equipment = new Role.Instance.Equip(player.Owner);
                                player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
                                player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
                                player.Owner.MySpells = new Role.Instance.Spell(player.Owner);

                                if (player.Owner.Achievement == null)
                                    player.Owner.Achievement = new Database.AchievementCollection();
                                Database.Server.GamePoll.TryAdd(user.Owner.Player.UID, user.Owner);


                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var msg = rec.GetStream();
                                    obj.Send(msg.MsgInterServerIdentifier(0, player.RealUID, New_UID));
                                }

                                user.Owner.Player.View.Role();
                                user.Owner.FullLoading = true;
                                SetLocation(user.Owner);

                                if (Program.ServerConfig.IsInterServer)
                                    user.Owner.Player.SetPkMode(Role.Flags.PKMode.Union);
                                else
                                    user.Owner.Player.SetPkMode(Role.Flags.PKMode.Peace);


                                user.Owner.Player.SendString(stream, (MsgStringPacket.StringID)60, false, null);

                                user.Owner.Player.ActiveSpecialTitles(stream);
                                user.Owner.Player.Stamina = 100;
                                user.Owner.Player.SendUpdate(stream, user.Owner.Player.Stamina, MsgUpdate.DataType.Stamina);

                            }
                            break;
                        }
                    case Game.GamePackets.Item:
                        {
                          
                                MsgGameItem item;
                                stream.GetItemPacketPacket(out item);
                                switch (item.Position)
                                {
                                    case 0:
                                        {

                                            user.Owner.Inventory.AddDBItem(item);
                                            if (user.Owner.Inventory.ClientItems.ContainsKey(item.UID))
                                            {
                                                if (user.Owner.Inventory.ClientItems[item.UID].StackSize != item.StackSize)
                                                    user.Owner.Inventory.ClientItems[item.UID] = item;
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            if (item.Position > 0 && item.Position <= (ushort)Role.Flags.ConquerItem.Wing)
                                            {
                                                user.Owner.Equipment.Add(item, stream);//.ClientItems.TryAdd(item.UID, item);
                                            }
                                            else if (item.Position > (ushort)Role.Flags.ConquerItem.Wing && item.Position <= (ushort)Role.Flags.ConquerItem.AleternanteGarment)
                                            {
                                                user.Owner.Equipment.ClientItems.TryAdd(item.UID, item);
                                            }
                                            break;
                                        }
                                }
                          
                            break;
                        }
                    case Game.GamePackets.ExtraItem:
                        {

                            MsgItemExtra.Refinery refinary;
                            MsgItemExtra.Purification purification;
                            stream.GetExtraItem(out purification, out refinary);
                            MsgGameItem Item;
                            if (user.Owner.TryGetItem(refinary.ItemUID, out Item))
                                Item.Refinary = refinary;
                            if (user.Owner.TryGetItem(purification.ItemUID, out Item))
                                Item.Purification = purification;
                            if (Item != null)
                                Item.Send(user.Owner, stream);
                            // user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);

                            break;
                        }
                    case Game.GamePackets.SubClass:
                        {
                            
                                MsgSubClass.Action action;
                                MsgSubClass.SubClases[] src;
                                stream.GetSubClass(out action, out src);
                                for (int x = 0; x < src.Length; x++)
                                    user.Owner.Player.SubClass.src.TryAdd(src[x].ID, src[x]);
                          
                            break;
                        }
                    case Game.GamePackets.MsgStauts:
                        {
                            
                                uint Type;
                                uint UID;
                                Game.MsgServer.MsgStatus Jiang_status;
                                stream.GetJiangStatus(out Type, out UID, out Jiang_status);
                                if (Type == 1)
                                {
                                    user.Owner.Player.MyJiangHu = new Role.Instance.JiangHu(user.Owner.Player.UID)
                                    {
                                        Name = user.Owner.Player.Name,
                                        Level = (byte)user.Owner.Player.Level,
                                        statusclient = Jiang_status
                                    };
                                    //       user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);
                                }
                           
                            break;
                        }
                    case Game.GamePackets.ChiInfo:
                        {
                           
                                MsgChiInfo.Action type;
                                uint CriticalStrike;
                                uint SkillCriticalStrike;
                                uint Immunity;
                                uint Breakthrough;
                                uint Counteraction;
                                uint MaxLife;
                                uint AddAttack;
                                uint AddMagicAttack;
                                uint AddMagicDefense;
                                uint FinalAttack;
                                uint FinalMagicAttack;
                                uint FinalDefense;
                                uint FinalMagicDefense;
                                stream.GetChiHandler(out type, out CriticalStrike, out SkillCriticalStrike, out Immunity
                                    , out Breakthrough, out Counteraction, out MaxLife, out AddAttack
                                    , out AddMagicAttack, out AddMagicDefense, out FinalAttack, out FinalMagicAttack
                                    , out FinalDefense, out FinalMagicDefense);

                                if (user.Owner.Player.MyChi != null && type == MsgChiInfo.Action.InterServerStatus)
                                {
                                    user.Owner.Player.MyChi.CriticalStrike = CriticalStrike;
                                    user.Owner.Player.MyChi.SkillCriticalStrike = SkillCriticalStrike;
                                    user.Owner.Player.MyChi.Immunity = Immunity;
                                    user.Owner.Player.MyChi.Breakthrough = Breakthrough;
                                    user.Owner.Player.MyChi.Counteraction = Counteraction;
                                    user.Owner.Player.MyChi.MaxLife = MaxLife;
                                    user.Owner.Player.MyChi.AddAttack = AddAttack;
                                    user.Owner.Player.MyChi.AddMagicAttack = AddMagicAttack;
                                    user.Owner.Player.MyChi.AddMagicDefense = AddMagicDefense;
                                    user.Owner.Player.MyChi.FinalAttack = FinalAttack;
                                    user.Owner.Player.MyChi.FinalMagicAttack = FinalMagicAttack;
                                    user.Owner.Player.MyChi.FinalDefense = FinalDefense;
                                    user.Owner.Player.MyChi.FinalMagicDefense = FinalMagicDefense;
                                }
                           
                            //  user.Owner.Equipment.QueryEquipment(user.Owner.Equipment.Alternante);
                            break;
                        }
                    case Game.GamePackets.Spell:
                        {
                          
                                MsgSpell spell;
                                stream.GetSpell(out spell);
                                if (user.Owner.MySpells.ClientSpells.ContainsKey(spell.ID) == false)
                                    user.Owner.MySpells.ClientSpells.TryAdd(spell.ID, spell);
                        
                            break;
                        }
                    case Game.GamePackets.Proficiency:
                        {
                           
                                MsgProficiency prof;
                                stream.GetProficiency(out prof);
                                if (user.Owner.MyProfs.ClientProf.ContainsKey(prof.ID) == false)
                                    user.Owner.MyProfs.ClientProf.TryAdd(prof.ID, prof);
                          
                            break;
                        }
                    default:
                        {
                            try
                            {

#if TEST
                            MyConsole.WriteLine("Receive -> PacketID: " + PacketID);
#endif

                                Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                                if (Program.MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                                {
                                    hinvoker(user.Owner, stream);
                                }
                                else
                                {
#if TEST
                                MyConsole.WriteLine("Not found the packet ----> " + PacketID);
#endif
                                }

                            }
                            catch (Exception e) { Console.WriteLine("hhh "+PacketID); MyConsole.WriteException(e); }
                            
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(PacketID);
                MyConsole.SaveException(e);
            }
            finally
            {
                ServerSockets.PacketRecycle.Reuse(stream);
            }
        }
        public static void SetLocation(Client.GameClient user)
        {
            switch (user.Player.SetLocationType)
            {
                case 1://elite pk
                    {
                        Game.MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(user);
                        break;
                    }
                default:
                    {
                        if (Database.GroupServerList.MyServerInfo.ID == Database.GroupServerList.InterServer.ID)
                        {
                            foreach (var server in Database.GroupServerList.GroupServers.Values)
                            {
                                if (server.ID == user.Player.ServerID)
                                {
                                    user.Teleport((ushort)server.X, (ushort)server.Y, (ushort)server.MapID);
                                }
                            }
                        }
                        else
                        {
                            user.Teleport(432, 390, 1002);
                        }
                        break;
                    }
            }
            return;
            foreach (var server in PollServers.Values)
            {
                if (server.ServerInfo.ID == user.Player.ServerID)
                {
                    user.Teleport((ushort)server.ServerInfo.X, (ushort)server.ServerInfo.Y, (ushort)server.ServerInfo.MapID);
                    break;
                }
            }
        }
        public static void Disconnect(ServerSockets.SecuritySocket obj)
        {
            var user = obj.Client as User;
            if (user.IsServer == false)
            {
                if (user.Owner.Map != null && user.Owner.Player != null)
                {
                    if (user.Owner.Player.SetLocationType == 1)//elitepk
                    {
                            user.Owner.EndQualifier();
                    }
                    user.Owner.Map.Denquer(user.Owner);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        user.Owner.Player.View.Clear(stream);
                    }
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryRemove(user.Owner.Player.UID, out client))
                    {
                        if (client.Player.InitTransfer == 2)
                        {
                            client.Player.InitTransfer = 0;
                            client.ClientFlag |= Client.ServerFlag.Disconnect;
                            client.ClientFlag |= Client.ServerFlag.QueuesSave;
                            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                        }
                    }

                }
            }
            user.Disconnect();
        }
    }
}
