﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
   public static class MsgJiangMessage
    {
       public static unsafe ServerSockets.Packet CreateJiangMessage(this ServerSockets.Packet stream, uint SpellID)
       {
           stream.InitWriter();

           stream.Write((ushort)SpellID);


           stream.Finalize(GamePackets.MsgJiangMessage);
           return stream;
       }
       
    }
}
