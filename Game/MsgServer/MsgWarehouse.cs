﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Project_Terror_v2.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetWarehouse(this ServerSockets.Packet stream, out uint NpcID, out MsgWarehouse.DepositActionID Action, out uint ItemUID)
        {
            NpcID = stream.ReadUInt32();
            uint unknow = stream.ReadUInt32();
            Action = (MsgWarehouse.DepositActionID)stream.ReadUInt32();
            uint file_size = stream.ReadUInt32();
            ItemUID = stream.ReadUInt32();


        }

        public static unsafe ServerSockets.Packet WarehouseCreate(this ServerSockets.Packet stream, uint NpcID, MsgWarehouse.DepositActionID Action, uint ItemUID, int File_Size, int count)
        {
            stream.InitWriter();
            stream.Write(NpcID);
            stream.Write((uint)0);
            stream.Write((uint)Action);
            stream.Write(File_Size);
            stream.Write(ItemUID);
            stream.Write(count);
            return stream;
        }

        public static unsafe ServerSockets.Packet AddItemWarehouse(this ServerSockets.Packet stream, Game.MsgServer.MsgGameItem item)
        {
            stream.Write(item.UID);
            stream.Write(item.ITEM_ID);
            stream.Write((byte)0);
            stream.Write((byte)item.SocketOne);
            stream.Write((byte)item.SocketTwo);
            stream.Write((uint)0);
            stream.Write((ushort)0);
            stream.Write(item.Plus);
            stream.Write(item.Bless);
            stream.Write(item.Bound);
            stream.Write(item.Enchant);
            stream.Write((byte)item.ProgresGreen);
            stream.Write((uint)item.Effect);
            stream.Write((byte)item.Locked);
            stream.Write((byte)item.Color);
            stream.Write(item.SocketProgress);
            stream.Write(item.PlusProgress);
            stream.Write(item.Inscribed);
            stream.Write((uint)(item.TimeLeftInMinutes * 60));//here is in secounds
            stream.Write((uint)0);
            stream.Write(item.StackSize);
            stream.Write(item.Purification.PurificationItemID);
            //   stream.Write(item.Durability);
        //    stream.Write(item.MaximDurability);
         //   stream.Write((ushort)0);
            stream.Write((ushort)0);
         

            stream.Write(item.PerfectionLevel);//level max = 59
            stream.Write(item.PerfectionProgress);//progres
            stream.Write(item.OwnerUID);//uid

            stream.Write(item.OwnerName, 16);
            stream.Write(item.Signature, 32);
            stream.ZeroFill(4);//unknow..
        
            return stream;
        }
        public static unsafe ServerSockets.Packet FinalizeWarehouse(this ServerSockets.Packet stream)
        {
            stream.Finalize(GamePackets.Warehause);
            return stream;
        }
    }
    public class MsgWarehouse
    {
        public enum DepositActionID : ushort
        {
            Show = 2560,
            DepositItem = 2561,
            WithdrawItem = 2562,

            Show_WH_House = 5120,
            DepositItem_WH_House = 5121,
            WithdrawItem_WH_House = 5122,

            ShashShow = 7680,
            ShashDepositItem = 7681,
            ShashWithdrawItem = 7682,

            ShowInventorySash = 10240,
            InventorySashDepositItem = 10241,
            InventorySashWithdrawItem = 10242,
        }

      

        [PacketAttribute(GamePackets.Warehause)]
        public unsafe static void HandlerWarehause(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (!client.Player.IsCheckedPass)
                return;//cheater
              uint NpcID; 
            MsgWarehouse.DepositActionID Action;
            uint ItemUID;

            stream.GetWarehouse(out NpcID, out Action, out ItemUID);
            if (client.PokerInfo != null)
                return;

            switch (Action)
            {
                case DepositActionID.InventorySashDepositItem:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE, stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.DepositItem_WH_House:
                case DepositActionID.DepositItem:
                    {

                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID || client.Player.UID == NpcID)
                        {
                            MsgGameItem item;
                            if (client.Inventory.TryGetItem(ItemUID, out item))
                            {
                                if (client.Warehouse.AddItem(item, NpcID))
                                {
                                    client.Inventory.Update(item, Role.Instance.AddMode.REMOVE,stream, true);


                                    stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                    stream.AddItemWarehouse(item);

                                    client.Send(stream.FinalizeWarehouse());


                                    item.SendItemExtra(client, stream);
                                    item.SendItemLocked(client, stream);
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.ShashDepositItem:
                    {
                         MsgGameItem Shash;
                         if (client.Inventory.TryGetItem(NpcID, out Shash))
                         {
                             if (Database.ItemType.IsSash(Shash.ITEM_ID))
                             {
                                 MsgGameItem item;
                                 if (client.Inventory.TryGetItem(ItemUID, out item))
                                 {
                                     byte ShashCount = Database.ItemType.GetSashCounts(Shash.ITEM_ID);
                                     if (Shash.Deposite.Count < ShashCount)
                                     {
                                         if (Shash.Deposite.TryAdd(item.UID, item))
                                         {
                                             client.Inventory.Update(item, Role.Instance.AddMode.REMOVE,stream);


                                             stream.WarehouseCreate(NpcID, Action, 0, 0, 1);

                                             stream.AddItemWarehouse(item);

                                             client.Send(stream.FinalizeWarehouse());

                                             item.SendItemExtra(client, stream);
                                             item.SendItemLocked(client, stream);
                                         }

                                     }
                                 }
                             }
                         }
                        break;
                    }
       
                case DepositActionID.ShashShow:
                    {
                        MsgGameItem Shash;
                        if (client.Inventory.TryGetItem(NpcID, out Shash))
                        {

                            if (Database.ItemType.IsSash(Shash.ITEM_ID))
                            {


                                stream.WarehouseCreate(NpcID, Action, 0, Database.ItemType.GetSashCounts(Shash.ITEM_ID), Shash.Deposite.Count);

                                foreach (var item in Shash.Deposite.Values)
                                    stream.AddItemWarehouse(item);

                                client.Send(stream.FinalizeWarehouse());



                                Game.MsgServer.MsgItemExtra itemExtra = new Game.MsgServer.MsgItemExtra();

                                var Array = Shash.Deposite.Values.ToArray();
                                for (byte x = 0; x < Array.Length; x++)
                                {
                                    var item = Array[x];
                                    if (item.Refinary.InLife)
                                    {
                                        item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.RefinaryAdding;
                                        if (item.Refinary.EffectDuration == 0)
                                            item.Refinary.Typ = Game.MsgServer.MsgItemExtra.Typing.PermanentRefinery;
                                        itemExtra.Refinerys.Add(item.Refinary);
                                    }
                                    if (item.Purification.InLife)
                                    {
                                        item.Purification.Typ = Game.MsgServer.MsgItemExtra.Typing.PurificationAdding;
                                        itemExtra.Purifications.Add(item.Purification);
                                    }
                                }

            

                                if (itemExtra.Refinerys.Count != 0 || itemExtra.Purifications.Count != 0)
                                    client.Send(itemExtra.CreateArray(stream));
                            }
                        }

                        break;
                    }
                case DepositActionID.ShowInventorySash:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            client.Warehouse.Show(NpcID, Action, stream);
                        }
                        break;
                    }
                case DepositActionID.Show_WH_House:
                case DepositActionID.Show:
                    {
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            client.Warehouse.Show(NpcID, Action,stream);
                        }
                        break;
                    }
                case DepositActionID.ShashWithdrawItem:
                    {
                        MsgGameItem Shash;
                        if (client.Inventory.TryGetItem(NpcID, out Shash))
                        {
                            if (Database.ItemType.IsSash(Shash.ITEM_ID))
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    MsgGameItem item;
                                    if (Shash.Deposite.TryRemove(ItemUID, out item))
                                    {
                                        item.WH_ID = 0;
                                        item.Position = 0;
                                        client.Inventory.Update(item, Role.Instance.AddMode.ADD, stream);

                                        stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 1);

                                        client.Send(stream.FinalizeWarehouse());
                                    }
                                }
                                else
                                {
#if Arabic
                                         client.SendSysMesage("Your Inventory Is Full!");
#else
                                    client.SendSysMesage("Your Inventory Is Full!");
#endif
                               
                                }
                            }
                        }
                        break;
                    }
                case DepositActionID.InventorySashWithdrawItem:
                    {
                        if (client.Player.UID == NpcID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID, stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
                case DepositActionID.WithdrawItem_WH_House:
                case DepositActionID.WithdrawItem:
                    {
                        if (Role.Instance.Warehouse.IsWarehouse((MsgNpc.NpcID)NpcID) || client.Player.UID == client.Player.DynamicID)
                        {
                            if (client.Warehouse.RemoveItem(ItemUID, NpcID,stream))
                            {
                                stream.WarehouseCreate(NpcID, Action, ItemUID, 0, 0);

                                client.Send(stream.FinalizeWarehouse());
                            }
                        }
                        break;
                    }
            }
        }


    }
}
