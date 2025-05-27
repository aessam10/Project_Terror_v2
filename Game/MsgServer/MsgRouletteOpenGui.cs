﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteOpenGuiCreate(this ServerSockets.Packet stream, MsgRouletteTable.TableType Type
            , MsgRouletteOpenGui.Color PlayerColor, byte TimerStamp, Database.Roulettes.RouletteTable.Member[] Members = null)
        {
            stream.InitWriter();

            stream.Write((uint)Type);
            stream.Write((byte)PlayerColor);
            stream.Write(TimerStamp);
            if (Members != null)
            {
                stream.Write((byte)Members.Length);

                foreach (var member in Members)
                {
                    stream.Write(member.Owner.Player.UID);
                    stream.Write(member.Owner.Player.Mesh);
                    stream.Write((byte)member.Color);
                    stream.Write(member.Owner.Player.Name, 16);

                }
            }
            else
                stream.Write((byte)0);
            stream.Finalize(GamePackets.MsgRouletteOpenGui);
            return stream;
        }
    }

    public unsafe struct MsgRouletteOpenGui
    {
        public enum Color : byte
        {
            Blue = 0,
            Yellow = 1,
            Green = 2,
            Mauve = 3,
            Gray = 4,
            None =5,
            Watch = 99//not sure
        }
    }
}
