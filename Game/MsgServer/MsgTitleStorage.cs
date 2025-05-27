using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Project_Terror_v2.Game.MsgServer
{
   public static class MsgTitleStorage
    {
       public enum TitleType : uint
       {
           Overlord = 1,
           RisingStar = 2001,
           Victor = 2002,
           Conqueror = 2003,
           Talent = 2004,
           Fashionist = 2005,
           SwiftChaser = 2006,
           MonkeyRider = 2013,
           SolarRider = 2014,
           LunarRider = 2015,
           SaintRider = 2016,
           Grandmaster = 2018,
           Fairy = 2020,
           Goddess = 2021,
           Beauty = 2022,
           Scholarz = 2023,
           Handsome = 2024,
           Wise = 2025,
           Superman = 2026,
           Scholar = 2027,
           EarthKnight = 2028,
           GloryKnight = 2029,
           SkyKnight = 2030,
           Paladin = 2031,
           BigFan = 2032,
           EuroCollector = 2033,
           Invincible = 2034,

           Legendary = 2035,
           Peerless = 2036,
           Outstanding = 2037,
           Expert = 2038,

           //wings
           WingsofSolarDra = 4001,
           WingsofInfernal = 6001,
           RadiantWings = 6002,
           StarlightWings = 6003,
           MoonlightWings = 6004,
           FairyWings = 6005,
           VioletCloudWing = 6007,
           VioletLightning = 6008,
           WingsofPlanet = 6009,
           Supreme = 6011

       }
       [ProtoContract]
       public class TitleStorage
       {
           [ProtoMember(1, IsRequired = true)]
           public Action ActionID;
           [ProtoMember(2, IsRequired = true)]
           public uint dwparam1;
           [ProtoMember(3, IsRequired = true)]
           public uint dwparam2;
           [ProtoMember(4, IsRequired = true)]
           public uint dwparam3;
           [ProtoMember(5, IsRequired = true)]
           public Title Title;
       }
       [ProtoContract]
       public class Title
       {
           [ProtoMember(1, IsRequired = true)]
           public uint ID;
           [ProtoMember(2, IsRequired = true)]
           public uint SubId;
           [ProtoMember(3, IsRequired = true)]
           public uint dwparam1;//active or score??
           [ProtoMember(4, IsRequired = true)]
           public uint dwparam2;
       }
       [Flags]
       public enum Action : uint
       {
           UpdateScore = 0,
           UseTitle =1,
           RemoveTitle = 3,
           Equip = 4,
           UnEquip = 5,
           FullLoad = 6
       }
       public static unsafe ServerSockets.Packet CreateTitleStorage(this ServerSockets.Packet stream, TitleStorage obj)
       {
           stream.InitWriter();
           stream.ProtoBufferSerialize(obj);
           stream.Finalize(GamePackets.MsgTitleStorage);
           return stream;
       }
       public static unsafe void GetTitleStorage(this ServerSockets.Packet stream, out TitleStorage pQuery)
       {
           pQuery = new TitleStorage();
           pQuery = stream.ProtoBufferDeserialize<TitleStorage>(pQuery);
       }
       [PacketAttribute(GamePackets.MsgTitleStorage)]
       private unsafe static void Process(Client.GameClient client, ServerSockets.Packet stream)
       {
           TitleStorage pQuery;
           stream.GetTitleStorage(out pQuery);
         
           switch (pQuery.ActionID)
           {
               case Action.Equip:
                   {
                       if (client.Player.SpecialTitles.Contains((TitleType)pQuery.dwparam2))
                       {
                           Database.TitleStorage dbtitle;
                           if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                           {
                               if (dbtitle.ID <= 4001)
                               {
                                   client.Player.SpecialTitleScore = dbtitle.Score;
                                   client.Player.SpecialTitleID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);
                               }
                               else
                                   client.Player.SpecialWingID = (uint)(dbtitle.ID * 10000 + dbtitle.SubID);

                               client.Send(stream.CreateTitleStorage(pQuery));
                               pQuery.ActionID = Action.UseTitle;
                               pQuery.Title = new Title();
                               pQuery.Title.ID = pQuery.dwparam2;
                               pQuery.Title.SubId = pQuery.dwparam3;
                               if (dbtitle.ID > 4001)
                                   pQuery.Title.dwparam1 = 1;
                               else
                                   pQuery.Title.dwparam1 = dbtitle.Score;
                               client.Send(stream.CreateTitleStorage(pQuery));
                               client.Player.View.SendView(client.Player.GetArray(stream, false),false);
                           }
                       }
                       break;
                   }
               case Action.UnEquip:
                   {
                       if (client.Player.SpecialTitles.Contains((TitleType)pQuery.dwparam2))
                       {
                           Database.TitleStorage dbtitle;
                           if (Database.TitleStorage.Titles.TryGetValue(pQuery.dwparam2, out dbtitle))
                           {
                               if (dbtitle.ID <= 4001)
                                   client.Player.SpecialTitleScore = client.Player.SpecialTitleID = 0;
                               else
                                   client.Player.SpecialWingID = 0;

                               client.Send(stream.CreateTitleStorage(pQuery));
                               client.Player.View.Clear(stream);
                               client.Player.View.Role(false);
                               client.Player.View.SendView(client.Player.GetArray(stream, false), false);
                           }
                       }
                       break;
                   }
           }
       }

    }
}
