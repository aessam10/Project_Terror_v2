﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer.AttackHandler
{
   public class BreathFocus
    {
        public unsafe static void Execute(InteractQuery Attack, Client.GameClient user, ServerSockets.Packet stream , Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, user.Player.X, user.Player.Y, (ushort)Role.Flags.SpellID.BreathFocus
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);
                if (user.Player.Stamina < 100)
                {
                    user.Player.Stamina = (ushort)Math.Min((int)(user.Player.Stamina + 20), 100);
                    user.Player.SendUpdate(stream, user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                }
                else if (user.Player.Stamina > 100)
                {
                    user.Player.Stamina = (ushort)Math.Min((int)(user.Player.Stamina + 20), 150);
                    user.Player.SendUpdate(stream,user.Player.Stamina, Game.MsgServer.MsgUpdate.DataType.Stamina);
                }
                //MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(obj.client.Player.UID, 0, MsgAttackPacket.AttackEffect.None));

                Updates.UpdateSpell.CheckUpdate(stream,user,Attack, 1000, DBSpells);

                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

            }
        }
    }
}
