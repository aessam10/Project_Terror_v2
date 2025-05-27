using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.WebServer
{
    public static class Packets
    {
        public const ushort LoginQueues = 10000
            , Closed = 10001
            , Ping = 11000
            , UpdatePlayersRanks = 10002
            , UpdateGwWinner = 10003
            , UpdateDb = 10004
            , CheckPrize = 10005
            , CheckRecharge = 10007
            , UpdateCTF = 10006;
      
        public unsafe struct Login
        {
            public ushort Length;
            public ushort ID;
            public uint UID;

            public static Login Create()
            {
                Login ptr = new Login();
                ptr.Length = 8;
                ptr.ID = LoginQueues;
                return ptr;
            }
        }

        public unsafe struct CheckAlive
        {
            public ushort Length;
            public ushort ID;
            public uint Online;

            public static CheckAlive Create()
            {
                CheckAlive ptr = new CheckAlive();
                ptr.Length = 8;
                ptr.ID = Ping;
                return ptr;
            }
        }

        public unsafe struct Close
        {
            public ushort Length;
            public ushort ID;

            public static Close Create()
            {
                Close ptr = new Close();
                ptr.Length = 8;
                ptr.ID = Closed;
                return ptr;
            }
        }

        public unsafe struct PlayersRanks
        {
            public ushort Length;
            public ushort ID;
            public Database.RanksTable.TopType RankType;
            public uint Count;
            public Database.RanksTable.Item Array;

            public static PlayersRanks* Create(uint count)
            {
                byte[] Buff = new byte[12 + 32 * count];
                fixed (byte* pointer = Buff)
                {
                    PlayersRanks* ptr = (PlayersRanks*)pointer;
                    ptr->Length = (ushort)Buff.Length;
                    ptr->Count = count;
                    ptr->ID = UpdatePlayersRanks;
                    return ptr;
                }
            }

            public byte* FinalizePacket(PlayersRanks* ptr, uint Count)
            {
                ptr->Length = (ushort)(12 + 32 * Count);
                return (byte*)ptr;
            }

        }
        public unsafe struct UpdateGuildInfo
        {
            public ushort Length;
            public ushort ID;

            public fixed sbyte szLeaderName[16];
            public fixed sbyte szGuildName[16];

            public string LeaderName
            {
                set
                {
                    fixed (sbyte* ptr = szLeaderName)
                    {
                        for (int x = 0; x < value.Length; x++)
                            *(sbyte*)(ptr + x) = (sbyte)value[x];
                    }
                }
            }

            public string GuildName
            {
                set
                {
                    fixed (sbyte* ptr = szGuildName)
                    {
                        for (int x = 0; x < value.Length; x++)
                            *(sbyte*)(ptr + x) = (sbyte)value[x];
                    }
                }
            }

            public static UpdateGuildInfo Create()
            {
                UpdateGuildInfo msg = new UpdateGuildInfo();
                msg.Length = 36;
                msg.ID = Packets.UpdateGwWinner;
                return msg;
            }

        }

        public struct PrizeDonation
        {
            public ushort Length;
            public ushort PacketID;
            public uint UserUID;
            public uint PrizeUID;
            public uint PrizeID;
            public uint Claim;

            public static PrizeDonation Create()
            {
                PrizeDonation msg = new PrizeDonation();
                msg.Length = 20;
                msg.PacketID = CheckPrize;

                return msg;
            }
        }
    }
}
