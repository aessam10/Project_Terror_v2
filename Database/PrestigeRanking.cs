using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Database
{
    public class PrestigeRanking
    {
        public enum Type : byte
        {
            Trojan=0,
            Warrior = 1,
            Archer =2,
            Ninja = 3,
            Monk = 4,
            Pirate = 5,
            DragonWarrior = 6,
            WaterTao = 7,
            FireTao = 8,
            WindWalker = 9,// xD
            Count = 10
        }

        public static Type GetIndex(byte Class)
        {
            if (Database.AtributesStatus.IsTrojan(Class))
                return Type.Trojan;
            if (Database.AtributesStatus.IsWarrior(Class))
                return Type.Warrior;
            if (Database.AtributesStatus.IsArcher(Class))
                return Type.Archer;
            if (Database.AtributesStatus.IsNinja(Class))
                return Type.Ninja;
            if (Database.AtributesStatus.IsMonk(Class))
                return Type.Monk;
            if (Database.AtributesStatus.IsPirate(Class))
                return Type.Pirate;
            if (Database.AtributesStatus.IsLee(Class))
                return Type.DragonWarrior;
            if (Database.AtributesStatus.IsWater(Class))
                return Type.WaterTao;
            if (Database.AtributesStatus.IsFire(Class))
                return Type.FireTao;
            if (Database.AtributesStatus.IsWindWalker(Class))
                return Type.WindWalker;
            return Type.WindWalker;
        }
        public class Entry
        {
            public uint Rank;
            public uint UID;
            public uint[] Points = new uint[20];
            public uint TotalPoints = 0;
            public byte Class;
            public byte Level;
            public Type type;
            public uint Mesh;
            public string Name = "";

            public uint HairStyle;
            public uint Head;
            public uint Garment;
            public uint LeftWeapon;
            public uint LefttWeaponAccessory;
            public uint RightWeapon;
            public uint RightWeaponAccessory;
            public uint MountArmor;
            public uint Armor;//??
            public uint Wing;
            public uint WingPlus;
            public uint Title;
            public uint Flag;//??
            public string GuildName = "";

            public void AddInfo(Client.GameClient user)
            {
                HairStyle = user.Player.Hair;
                Head = user.Equipment.HeadID;
                Garment = user.Player.GarmentId;
                LeftWeapon = user.Equipment.LeftWeapon;
                RightWeapon = user.Equipment.RightWeapon;
                RightWeaponAccessory = user.Player.RightWeaponAccessoryId;
                LefttWeaponAccessory = user.Player.LeftWeaponAccessoryId;
                MountArmor = user.Player.MountArmorId;
                Wing = user.Player.WingId;
                if (user.Player.SpecialWingID != 0)
                    Wing = user.Player.SpecialWingID;
                WingPlus = user.Player.WingPlus;
                Title = user.Player.SpecialTitleID;
                Armor = user.Player.ArmorId;
                if(user.Player.MyGuild != null)
                GuildName = user.Player.MyGuild.GuildName;

                if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.WeeklyPKChampion;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopTrojan))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopTrojan;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWarrior))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWarrior;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopArcher))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopArcher;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopNinja))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopNinja;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopMonk))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopMonk;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopPirate))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopPirate;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDragonLee))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDragonLee;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopFireTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopFireTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWaterTaoist;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWar))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopSuperGuildWar;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopDeputyLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopGuildLeader))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopGuildLeader;
                else if (user.Player.ContainFlag(Game.MsgServer.MsgUpdate.Flags.TopWindWalker))
                    Flag = (uint)Game.MsgServer.MsgUpdate.Flags.TopWindWalker;

            }
        }
       
        public static Entry BestOfTheWorld = null;
        public static Extensions.SafeDictionary<Type, Rank> Ranks = new Extensions.SafeDictionary<Type, Rank>();
        public static object SynRoot = new object();


        public static void Create()
        {
            for (int x = 0; x < (byte)Type.Count; x++)
                Ranks.Add((Type)x, new Rank((Type)x));
        }

        public class Rank
        {
            public Entry BestOfClass;
            public Type _Type;
            public  Extensions.SafeDictionary<uint, Entry> Items = new Extensions.SafeDictionary<uint, Entry>();

            public Rank(Type typ)
            {
                _Type = typ;
            }
            public const int MaxItems = 30;
            private  Entry[] Top30 = new Entry[0];
            public  Entry[] GetRank30()
            {
                lock (Top30)
                    return Top30;
            }
            public  void AddItem(Type typ, Entry item)
            {
                if (BestOfClass == null || BestOfClass.UID == item.UID)
                    BestOfClass = item;
                if (item.TotalPoints > BestOfClass.TotalPoints)
                    BestOfClass = item;

                if (BestOfTheWorld == null || BestOfClass.UID == item.UID)
                    BestOfTheWorld = item;
                if (item.TotalPoints > BestOfTheWorld.TotalPoints)
                    BestOfTheWorld = item;
                if (Items.Count < MaxItems)
                {
                    if (!Items.ContainsKey(item.UID))
                    {
                        Items.Add(item.UID, item);
                        lock (SynRoot)
                        {
                            Top30 = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                            for (int x = 0; x < Top30.Length; x++)
                                Top30[x].Rank = (ushort)(x + 1);
                        }
                    }
                    else if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].TotalPoints;
                        uint rank = Items[item.UID].Rank;
                        Items[item.UID] = item;
                        item.Rank = rank;
                        if (points != item.TotalPoints)
                        {
                            lock (SynRoot)
                            {
                                Top30 = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                for (int x = 0; x < Top30.Length; x++)
                                    Top30[x].Rank = (ushort)(x + 1);
                            }
                        }
                    }
                }
                else
                {
                    if (Items.ContainsKey(item.UID))
                    {
                        uint points = Items[item.UID].TotalPoints;
                        uint rank = Items[item.UID].Rank;
                        Items[item.UID] = item;
                        item.Rank = rank;

                        if (points != item.TotalPoints)
                        {
                            lock (SynRoot)
                            {
                                Top30 = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                for (int x = 0; x < Top30.Length; x++)
                                    Top30[x].Rank = (ushort)(x + 1);
                            }
                        }
                    }
                    else
                    {
                        var last = Top30[Top30.Length - 1];
                        if (item.TotalPoints > last.TotalPoints)
                        {
                            Items.Remove(last.UID);
                            Items.Add(item.UID, item);

                            lock (SynRoot)
                            {
                                Top30 = Items.Values.OrderByDescending(p => p.TotalPoints).ToArray();
                                for (int x = 0; x < Top30.Length; x++)
                                    Top30[x].Rank = (ushort)(x + 1);
                            }
                        }
                    }
                }
            }
        }

        public static void CheckReborn(Client.GameClient user)
        {
            if (user.Player.Reborn == 2)
            {
                if (Ranks[GetIndex(user.Player.SecoundeClass)].Items.ContainsKey(user.Player.UID))
                    Ranks[GetIndex(user.Player.SecoundeClass)].Items.Remove(user.Player.UID);
            }
            else if (user.Player.Reborn == 1)
            {
                if (Ranks[GetIndex(user.Player.FirstClass)].Items.ContainsKey(user.Player.UID))
                    Ranks[GetIndex(user.Player.FirstClass)].Items.Remove(user.Player.UID);
            }
        }

        public static uint GetMyRank(Type typ, uint UID)
        {
            Entry item;
            if (Ranks[typ].Items.TryGetValue(UID, out item))
            {
                return item.Rank;
            }
            return ushort.MaxValue;
        }
        public static Entry GetInfo(uint UID)
        {
            foreach (var rank in Ranks.Values)
            {
                foreach (var item in rank.Items.GetValues())
                    if (item.UID == UID)
                        return item;
            }
            return null;
        }


      
        public static void Load()
        {
            Create();
            using (DBActions.Read reader = new DBActions.Read("PrestigeRanking.txt", false))
            {
                if (reader.Reader())
                {
                    int count = reader.Count;
                    for (int x = 0; x < count; x++)
                    {
                        DBActions.ReadLine line = new DBActions.ReadLine(reader.ReadString("/"), '/');
                        Entry item = new Entry();
                        item.UID = line.Read((uint)0);
                        item.Name = line.Read("");
                        item.type =  (Type)line.Read((byte)0);

                        item.Class = line.Read((byte)0);
                        item.Level = line.Read((byte)0);
                        item.Mesh = line.Read((uint)0);
                        item.TotalPoints = line.Read((uint)0);
                        item.HairStyle = line.Read((uint)0);
                        item.Head = line.Read((uint)0);
                        item.Garment = line.Read((uint)0);
                        item.LeftWeapon = line.Read((uint)0);
                        item.LefttWeaponAccessory = line.Read((uint)0);
                        item.RightWeapon = line.Read((uint)0);
                        item.RightWeaponAccessory = line.Read((uint)0);
                        item.MountArmor = line.Read((uint)0);
                        item.Armor = line.Read((uint)0);
                        item.Wing = line.Read((uint)0);
                        item.WingPlus = line.Read((uint)0);
                        item.Title = line.Read((uint)0);
                        item.Flag = line.Read((uint)0);
                        item.GuildName = line.Read("");
                        int pointscount = line.Read((int)0);
                        for (int i = 0; i < pointscount; i++)
                        {
                            item.Points[i] = line.Read((uint)0);
                           
                        }
                       Ranks[item.type].AddItem(item.type,item);
                    }
                }
            }
        }
        public static void Save()
        {
            using (DBActions.Write writer = new DBActions.Write("PrestigeRanking.txt"))
            {
                foreach (var rank in Ranks.GetValues())
                {
                    foreach (var obj in rank.Items.GetValues())
                    {
                        Database.DBActions.WriteLine line = new DBActions.WriteLine('/');
                        line.Add(obj.UID).Add(obj.Name).Add((byte)obj.type).Add(obj.Class)
                            .Add(obj.Level).Add(obj.Mesh).Add(obj.TotalPoints)
                            .Add(obj.HairStyle)
                            .Add(obj.Head)
                            .Add(obj.Garment)
                            .Add(obj.LeftWeapon)
                            .Add(obj.LefttWeaponAccessory)
                            .Add(obj.RightWeapon)
                            .Add(obj.RightWeaponAccessory)
                            .Add(obj.MountArmor)
                            .Add(obj.Armor)
                            .Add(obj.Wing)
                            .Add(obj.WingPlus)
                            .Add(obj.Title)
                            .Add(obj.Flag)
                        .Add(obj.GuildName);
                      
                        if (obj.Points == null)
                            line.Add(0);
                        else
                        {
                            line.Add(obj.Points.Length);
                            for (int x = 0; x < obj.Points.Length; x++)
                                line.Add(obj.Points[x]);
                        }
                        writer.Add(line.Close());
                    }
                }
                writer.Execute(DBActions.Mode.Open);
            }
        }
    }
}
