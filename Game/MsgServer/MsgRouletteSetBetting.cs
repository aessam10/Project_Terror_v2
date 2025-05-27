﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet RouletteShareBettingCreate(this ServerSockets.Packet stream, Database.Roulettes.RouletteTable.Member player,MsgRouletteOpenGui.Color color)
        {
            stream.InitWriter();

            stream.Write((byte)color);

            var MyLuckNumbers = player.MyLuckNumber.Values.ToArray();
            var MyLuckExtras = player.MyLuckExtra.Values.ToArray();

            stream.Write((byte)(MyLuckNumbers.Length + MyLuckExtras.Length));


            foreach (var element in MyLuckNumbers)
            {
                stream.Write(element.Number);
                stream.Write(element.BetPrice);
            } 
            foreach (var element in MyLuckExtras)
            {
                stream.Write(element.Number);
                stream.Write(element.BetPrice);
            }

            stream.Finalize(GamePackets.MsgRouletteShareBetting);
            return stream;
        }
    }
}
