﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgMonster
{
    [Flags]
    public enum MobStatus : uint
    {
        Idle = 0,
        SearchTarget = 1,
        Attacking = 2,
        Respawning = 3
    }
}
