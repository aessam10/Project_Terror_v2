using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public class MsgCheckLine
    {
        public static bool CheckLineSpells(Client.GameClient user)
        {
            if (user.MySpells.ClientSpells.ContainsKey(1045))
                return true;
            if (user.MySpells.ClientSpells.ContainsKey(1046))
                return true;


                if (user.MySpells.ClientSpells.ContainsKey(12350))
                return true;


            return false;
        }

        public static bool CheckItems(Client.GameClient user)
        {

            Game.MsgServer.MsgGameItem RightWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out RightWeapon))
            {
                if (CheckItem(RightWeapon.ITEM_ID))
                    return true;
            }

            Game.MsgServer.MsgGameItem LeftWeapon;
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out LeftWeapon))
            {
                if (CheckItem(LeftWeapon.ITEM_ID))
                    return true;
            }//tu ii lasi pe toti? e aiurea asta .. te forteaza de prea multe chestii .. ok
#if Encore
             return true;	
#else
            return false;
#endif

        }

        public static bool CheckItem(uint ID)
        {
            if (Database.ItemType.IsTrojanEpicWeapon(ID) || Database.ItemType.IsTaoistEpicWeapon(ID))
                return true;
            return ID >= 410003 && ID <= 410439 || ID >= 420003 && ID <= 420439 || ID >= 421003 && ID <= 421439
                || ID == 410501 || ID == 410601 || ID == 410701 || ID == 410801 || ID == 410901;

        }
    }
}
