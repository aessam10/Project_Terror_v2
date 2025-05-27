﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
   public static class MsgLeagueInfo
    {
       public static unsafe ServerSockets.Packet LeagueInfoCreate(this ServerSockets.Packet stream, ulong Treasury, uint GoldBricks
           , uint Stipend, string Name, string LeaderName, string Bulletin, string Title, string PlunderTarget, string InvadingUnion
           )
       {
           stream.InitWriter();
           stream.Write(Treasury);
           stream.Write(GoldBricks);
           stream.Write(Stipend);
           stream.Write(3);//serverid 1
           stream.Write(2);//serverid 2
           stream.Write(Name, 16);
           stream.Write(LeaderName, 16);
           stream.Write(Bulletin, 127);//think 255.
           stream.ZeroFill(129);//???
           stream.Write(Title, 10);
           stream.Write(PlunderTarget, 16);
           stream.Write(InvadingUnion, 16);

           stream.Finalize(GamePackets.LeagueInfo);
           return stream;
       }
    }
}
