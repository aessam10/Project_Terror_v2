using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Database
{
    public class TitleStorage
    {

        public uint ID = 0;
        public uint SubID = 0;
        public string Name = "";
        public uint Score = 0;

        public static Dictionary<uint, TitleStorage> Titles = new Dictionary<uint, TitleStorage>();

        public static void LoadDBInformation()
        {

            string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "title_type.txt");
            foreach (var bas_line in baseText)
            {
                var line = bas_line.Split(' ');
                TitleStorage title = new TitleStorage();
                title.ID = uint.Parse(line[0]);
                title.SubID = uint.Parse(line[1]);
                title.Name = line[2];
                title.Score = uint.Parse(line[7]);
                Titles.Add(title.ID, title);
            }
        }
#if Encore
        public static void CheckUpUser(Client.GameClient user, ServerSockets.Packet stream)
        {

        #region VIP 6

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Invincible))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Invincible, stream);
                }
            }
                else
                 {
                     if (!(user.Player.VipLevel == 6))
                         user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Invincible, stream);
                 }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Overlord))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Fairy))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fairy, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fairy, stream);
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Goddess))
            {
                if (user.Player.VipLevel == 6 && Role.Core.IsGirl(user.Player.Body))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Goddess, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Goddess, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Beauty))
            {
                if (user.Player.VipLevel == 6 && Role.Core.IsGirl(user.Player.Body))
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Beauty, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Beauty, stream);
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Handsome))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Handsome, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Handsome, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Wise))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Wise, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Wise, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight))
            {
                if (user.Player.VipLevel == 1)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 1))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.EarthKnight, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GloryKnight, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SkyKnight, stream);
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Paladin))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Paladin, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Paladin, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.FairyWings, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Supreme))
            {
                if (user.Player.VipLevel == 6)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Supreme, stream);
                }
            }
            else
            {
                if (!(user.Player.VipLevel == 6))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Supreme, stream);
            }







            #endregion

        #region Prestige 
            
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing))
            {
                if (user.PrestigeLevel >= 324)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing, stream);
                }
            }
            else
            {
                if (!(user.PrestigeLevel <= 324))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning))
            {
                if (user.PrestigeLevel >= 216)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning, stream);
                }
            }
            else
            {
                if (!(user.PrestigeLevel <= 216))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning, stream);
            }


            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Superman))
            {
                if (user.MyPrestigePoints >= 600000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
            }
            else
            {
                if (!(user.MyPrestigePoints <= 600000))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
            }
            else
            {
                if (!(user.MyPrestigePoints <= 400000))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
            }





            #endregion

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar))
            {
                if (user.Player.Achievement.Score() >= 300)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar, stream);
                }
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Victor))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }
           /* else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }*/
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                }
            }
            /*else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                if (!(Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID
                    && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
            }*/



            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
                }
            }
            /*else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                if (!(Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID
                    && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
            }*/





            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Talent))
            {
                if (user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }
       /*     else
            {
                if (!(user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }

            */

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster))
            {
                if (user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }
           /* else
            {
                if (!(user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }*/

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                    }
                }
            }
         /*   else
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (!(Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                    }
                }
            }
            */

            if (CoatStorage.AmountStarGarments(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
            }
           // else
             //   user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
            
            if (CoatStorage.AmountStarMount(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
            }
       //     else
           //     user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);

            if (CoatStorage.AmountStarGarments(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);
            }
         //   else
            //    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);

            if (CoatStorage.AmountStarMount(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);
            }
          //  else
           //     user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);




        }
#else

        public static void CheckUpUser(Client.GameClient user, ServerSockets.Packet stream)
        {

            #region Prestige

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing))
            {
                if (user.PrestigeLevel >= 324)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletCloudWing, stream);
                }
            }
          

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning))
            {
                if (user.PrestigeLevel >= 216)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.VioletLightning, stream);
                }
            }
         

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Superman))
            {
                if (user.MyPrestigePoints >= 600000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
            }
          

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
            }
          
            #endregion

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar))
            {
                if (user.Player.Achievement.Score() >= 300)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar, stream);
                }
            }

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Victor))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }
           /* else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RadiantWings, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }*/
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                }
            }
            /*else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                if (!(Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID
                    && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
            }*/






            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Talent))
            {
                if (user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }
       /*     else
            {
                if (!(user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }

            */

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster))
            {
                if (user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }
           /* else
            {
                if (!(user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }*/

            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                    }
                }
            }
         /*   else
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (!(Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID))
                    {
                        user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofInfernal, stream);
                    }
                }
            }
            */

            if (CoatStorage.AmountStarGarments(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
            }
           // else
             //   user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);

            if (user.Player.InUnion && user.Player.UnionMemeber.Rank == Role.Instance.Union.Member.MilitaryRanks.Emperor)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
            }
            else
            {
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Overlord, stream);
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.WingsofSolarDra, stream);
            }

            if (CoatStorage.AmountStarMount(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
            }
       //     else
           //     user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);

            if (CoatStorage.AmountStarGarments(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);
            }
         //   else
            //    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.StarlightWings, stream);

            if (CoatStorage.AmountStarMount(user, 5) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);
            }
          //  else
           //     user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MoonlightWings, stream);



            /*1 1000 Overlord 0 0 0 0 150 for emperor
 2001 2 RisingStar 0 0 0 0 300 complete 320 achievements
 2002 3 Victor 0 0 0 0 150 champion leader of CTF
 2003 4 Conqueror 0 0 0 0 300 conqueror champoin gl of CS CTF
 2004 5 Talent 0 0 0 0 150 jiang >= 81,000
 2005 6 Fashionist 0 0 0 0 150 own 5 garments of 4-star or above
 2006 7 SwiftChaser 0 0 0 0 150 own 5 mount armor of 4-star or above
 2018 1 Grandmaster 0 0 0 0 500 total chi > 1600
 4001 1000 WingsofSolarDra 0 0 0 0 0 for emperor
 6001 20 WingsofInfernal 0 0 0 0 0 for champion of elite pk
 6002 21 RadiantWings 0 0 0 0 0 for champion CS-CTF
 6003 22 StarlightWings 0 0 0 0 0 own a 5-star garment
 6004 23 MoonlightWings 0 0 0 0 0 own a 5-star mount armor.*/
        }
#endif
    }
}
