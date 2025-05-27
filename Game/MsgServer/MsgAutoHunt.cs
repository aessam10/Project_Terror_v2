using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
    public static class MsgAutoHunt
    {
        public static unsafe ServerSockets.Packet AutoHuntCreate(this ServerSockets.Packet stream, ushort type, ulong dwparam)
        {
            stream.InitWriter();

            stream.Write(type);
            stream.Write(dwparam);//341
            stream.Write(0);//ulong.MaxValue);
            stream.Write(0);//ulong.MaxValue);
            stream.Finalize(GamePackets.AutoHunt);
            return stream;
        }




        [PacketAttribute(GamePackets.AutoHunt)]
        private static unsafe void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (user.Player.Map == 1700)
            {
                user.SendSysMesage("Auto hunt is not available on this map.");
                return;
            }
          //  MyConsole.PrintPacketAdvanced(stream.Memory, stream.Size);
            if (user.Player.OnAutoHunt == false)
            {
                if (user.Player.VipLevel >= 6)
                {
                    if (user.Player.OnXPSkill() != MsgUpdate.Flags.Normal)
                        user.Player.RemoveFlag(user.Player.OnXPSkill());
                    user.Send(stream.AutoHuntCreate(0, 341));
                    user.Send(stream.AutoHuntCreate(1, 341));
                    user.Player.OnAutoHunt = true;
                }
            }
            else
            {

                user.Send(stream.AutoHuntCreate(3, 0));
                user.Player.OnAutoHunt = false;
            }
        }
    }
}
