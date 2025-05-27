﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{

        public unsafe static partial class MsgBuilder
        {
            public static unsafe ServerSockets.Packet InnerPowerGui(this ServerSockets.Packet stream, Role.Instance.InnerPower.Stage.NeiGong[] gongs)
            {
                stream.InitWriter();
                if (gongs != null)
                {
                    stream.Write((ushort)gongs.Length);
                    for (int x = 0; x < gongs.Length; x++)
                    {
                        var element = gongs[x];
                        stream.Write((byte)element.ID);
                        stream.Write((uint)element.Score);
                    }
                }
                stream.Finalize(GamePackets.InnerPowerGui);
                return stream;
            }
        }
    
}
