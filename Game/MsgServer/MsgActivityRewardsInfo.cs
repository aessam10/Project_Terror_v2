﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Project_Terror_v2.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet ActivityRewardsInfoCreate(this ServerSockets.Packet stream, MsgActivityRewardsInfo.Action mode, uint ActivityPoints)
        {
            stream.InitWriter();

            stream.Write((byte)mode);//4

            stream.Write((uint)2);//5
            stream.Write((uint)1);//9

            stream.Write(ActivityPoints); //13

            stream.Write((uint)2);//17
            stream.Write((uint)2);//unknow this // 21

            stream.Finalize(GamePackets.MsgActivityRewardsInfo);
            return stream;
        }


    }
    public unsafe struct MsgActivityRewardsInfo
    {
        public enum Action : byte
        {
            InitializeList = 1
        }
    }
}
