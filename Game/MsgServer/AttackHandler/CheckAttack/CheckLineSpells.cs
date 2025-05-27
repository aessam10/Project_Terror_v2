using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer.AttackHandler.CheckAttack
{
   public class CheckLineSpells
    {
       public static bool CheckUp(Client.GameClient user, ushort spellid)
       {
           if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.SkillTournament
              && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
           {
               if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
               {
                   if (spellid != 1045 && spellid != 1046 && spellid != 11005)
                   {
                       user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword)");
                       return false;
                   }
               }
           }
          else if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.DragonWar
               && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
           {
               if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
               {
                   if (spellid != 1045 && spellid != 1046 && spellid != 11005)
                   {
                       user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword)");
                       return false;
                   }
               }
           }
           else if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FootBall
             && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
           {
               if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
               {
                   if (spellid != 1045 && spellid != 1046 && spellid != 11005)
                   {
                       user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword)");
                       return false;
                   }
               }
           }
           else if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.FreezeWar
            && MsgTournaments.MsgSchedules.CurrentTournament.Process == MsgTournaments.ProcesType.Alive)
           {
               if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(user))
               {
                   if (spellid != 1045 && spellid != 1046 && spellid != 11005)
                   {
                       user.SendSysMesage("You have to use manual linear skills(FastBlade/ScentSword)");
                       return false;
                   }
               }
           }
           return true;

       }
    }
}
