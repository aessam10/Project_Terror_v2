﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Project_Terror_v2.Game.MsgServer
{
    public static class MsgCoatStorage
    {

        [ProtoContract]
        public class CoatStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public Action ActionID;
            [ProtoMember(2, IsRequired = false)]
            public uint dwparam1;
            [ProtoMember(3, IsRequired = false)]
            public uint dwpram2;
            [ProtoMember(4, IsRequired = true)]
            public uint dwpram3;
            [ProtoMember(5, IsRequired = false)]
            public ItemStorage Item;

            public void AddItem(Game.MsgServer.MsgGameItem item)
            {
                Item = new ItemStorage();
                Item.ItemUID = item.UID;
                Item.ItemID = item.ITEM_ID;
                Item.MaxDurability = Item.MinDurability = item.MaximDurability;
                Item.Stack = 1;
                Item.Plus = item.Plus;
                Item.Bless = item.Bless;
                Item.Type2 = Item.Type1 = 3;

            }
        }
        [ProtoContract]
        public class ItemStorage
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ItemUID;
            [ProtoMember(2, IsRequired = true)]
            public uint ItemID;
            [ProtoMember(3, IsRequired = true)]
            public uint dwparam3;
            [ProtoMember(4, IsRequired = true)]
            public uint dwparam4;
            [ProtoMember(5, IsRequired = true)]
            public uint dwparam5;
            [ProtoMember(6, IsRequired = true)]
            public uint dwparam6;
            [ProtoMember(7, IsRequired = true)]
            public uint dwparam7;
            [ProtoMember(8, IsRequired = true)]
            public uint Plus;
            [ProtoMember(9, IsRequired = true)]
            public uint Bless;
            [ProtoMember(10, IsRequired = true)]
            public uint Type1;//??
            [ProtoMember(11, IsRequired = true)]
            public uint dwparam11;
            [ProtoMember(12, IsRequired = true)]
            public uint dwparam12;
            [ProtoMember(13, IsRequired = true)]
            public uint dwparam13;
            [ProtoMember(14, IsRequired = true)]
            public uint dwparam14;
            [ProtoMember(15, IsRequired = true)]
            public uint Type2;
            [ProtoMember(16, IsRequired = true)]
            public uint dwparam16;
            [ProtoMember(17, IsRequired = true)]
            public uint dwparam17;
            [ProtoMember(18, IsRequired = true)]
            public uint dwparam18;
            [ProtoMember(19, IsRequired = true)]
            public uint TimeLeft;
            [ProtoMember(20, IsRequired = true)]
            public uint dwparam20;
            [ProtoMember(21, IsRequired = true)]
            public uint Stack;
            [ProtoMember(22, IsRequired = true)]
            public uint MinDurability;
            [ProtoMember(23, IsRequired = true)]
            public uint MaxDurability;
        }
        [Flags]
        public enum Action : uint
        {
            Equip = 1,
            TakeOff = 6,
            AddToWardRobe = 5,
            Expire = 8,
            Retrive = 2
        }
        public static unsafe ServerSockets.Packet CreateCoatStorage(this ServerSockets.Packet stream, CoatStorage obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCoatStorage);

            return stream;
        }
        public static unsafe void GetCoatStorage(this ServerSockets.Packet stream, out CoatStorage pQuery)
        {
            pQuery = new CoatStorage();
            pQuery = stream.ProtoBufferDeserialize<CoatStorage>(pQuery);
        }
        public static bool CheckGender(Client.GameClient user, uint itemid)
        {
            Database.ItemType.DBItem dbitem;
            if (Database.Server.ItemsBase.TryGetValue(itemid, out dbitem))
            {
                if (dbitem.Gender == 0)
                    return true;
                if (dbitem.Gender == 1)//boy
                {
                    if (Role.Core.IsBoy(user.Player.Body))
                        return true;
                }
                if (dbitem.Gender == 2)//female
                {
                    if (Role.Core.IsGirl(user.Player.Body))
                        return true;
                }
            }
            return false;
        }
        [PacketAttribute(GamePackets.MsgCoatStorage)]
        private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
        {
            CoatStorage pQuery;
            stream.GetCoatStorage(out pQuery);
            switch (pQuery.ActionID)
            {
                case Action.Equip:
                    {

                        Game.MsgServer.MsgGameItem item;
                        if (client.Inventory.TryGetItem(pQuery.dwparam1, out item))
                        {
                            if (!CheckGender(client, item.ITEM_ID))
                                break;
                            if (client.MyWardrobe.Contain(item.ITEM_ID))
                                return;
                            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment)
                            {
                                if (client.Player.SpecialGarment != 0)
                                {
#if Arabic
                                    client.SendSysMesage("Item can't be unequiped during the event.");
#else
                                    client.SendSysMesage("Item can't be unequiped during the event.");
#endif

                                    return;
                                }
                            }
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment || Position == (ushort)Role.Flags.ConquerItem.SteedMount)
                            {
                                client.MyWardrobe.AddItem(item);
                                client.MyWardrobe.SendItem(stream, item);
                                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                store.ActionID = MsgCoatStorage.Action.Equip;
                                store.dwparam1 = item.UID;
                                client.Send(stream.CreateCoatStorage(store));

                            }

                        }
                        break;
                    }
                case Action.AddToWardRobe:
                    {
                        Game.MsgServer.MsgGameItem item;

                        if (client.MyWardrobe.TryGetItem(pQuery.dwparam1, out item))
                        {
                            ushort Position = Database.ItemType.ItemPosition(item.ITEM_ID);
                            if (Position == (ushort)Role.Flags.ConquerItem.Garment || Position == (ushort)Role.Flags.ConquerItem.SteedMount)
                            {
                                if (client.Equipment.FreeEquip((Role.Flags.ConquerItem)Position))
                                {
                                    if (client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    {
                                        item.Position = (ushort)Position;
                                        client.Equipment.Add(item, stream);
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }
                                else//if is item on character
                                {
                                    if (client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream))
                                    {
                                        client.Equipment.Remove((Role.Flags.ConquerItem)Position, stream);
                                        item.Position = (ushort)Position;
                                        client.Equipment.Add(item, stream);
                                        item.Mode = Role.Flags.ItemMode.Update;
                                        item.Send(client, stream);
                                    }
                                }
                            }
                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            store.ActionID = MsgCoatStorage.Action.AddToWardRobe;
                            store.dwparam1 = item.UID;
                            store.dwpram2 = item.ITEM_ID;
                            client.Send(stream.CreateCoatStorage(store));
                        }
                        break;
                    }
                case Action.TakeOff:
                    {
                        Game.MsgServer.MsgGameItem item;
                        if (client.Equipment.TryGetEquip((Role.Flags.ConquerItem)pQuery.dwpram2, out item))
                        {
                            if (item.Position == (ushort)Role.Flags.ConquerItem.Garment)
                            {
                                if (client.Player.SpecialGarment != 0)
                                {
#if Arabic
                                       client.SendSysMesage("Item can't be unequiped during the event.");
#else
                                    client.SendSysMesage("Item can't be unequiped during the event.");
#endif

                                    return;
                                }
                            }
                            client.Equipment.Remove((Role.Flags.ConquerItem)item.Position, stream);

                            Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            store.ActionID = MsgCoatStorage.Action.TakeOff;
                            store.dwparam1 = item.UID;
                            store.dwpram2 = item.ITEM_ID;
                            client.Send(stream.CreateCoatStorage(store));

                            client.Equipment.QueryEquipment(client.Equipment.Alternante);
                        }
                        break;
                    }
                case Action.Retrive:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
#if Arabic
                               client.SendSysMesage("Please make 1 more space in your inventory.");
#else
                            client.SendSysMesage("Please make 1 more space in your inventory.");
#endif

                            break;
                        }
                        Game.MsgServer.MsgGameItem item2;
                        if (client.Equipment.TryGetValue(pQuery.dwparam1, out item2))
                        {
                            if (item2.UID == pQuery.dwparam1)
                            {
                                client.Equipment.Remove((Role.Flags.ConquerItem)item2.Position, stream);

                                Game.MsgServer.MsgCoatStorage.CoatStorage store = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                                store.ActionID = MsgCoatStorage.Action.TakeOff;
                                store.dwparam1 = item2.UID;
                                store.dwpram2 = item2.ITEM_ID;
                                client.Send(stream.CreateCoatStorage(store));

                                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                            }
                        }

                        Game.MsgServer.MsgGameItem item;
                        if (client.MyWardrobe.RemoveItem(pQuery.dwparam1, out item))
                        {

                            client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);
                            Game.MsgServer.MsgCoatStorage.CoatStorage astore = new Game.MsgServer.MsgCoatStorage.CoatStorage();
                            astore.ActionID = MsgCoatStorage.Action.Retrive;
                            astore.dwparam1 = pQuery.dwparam1;
                            astore.dwpram2 = item.ITEM_ID;
                            client.Send(stream.CreateCoatStorage(astore));
                            client.Send(stream.CreateCoatStorage(astore));

                        }
                        break;
                    }
            }
        }

    }
}
