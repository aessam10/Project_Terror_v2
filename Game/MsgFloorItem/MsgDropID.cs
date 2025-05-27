using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgFloorItem
{
    public enum MsgDropID : ushort
    {
        Visible = 0x01,
        Remove = 0x02,
        Pickup = 0x03,
        DropDetain = 4,
        Effect = 10,
        RemoveEffect = 12,
        Earth = 13
    }
}
