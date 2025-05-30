﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Project_Terror_v2.MsgInterServer.Packets;

namespace Project_Terror_v2.Game.MsgServer
{
    using ActionInvoker = CachedAttributeInvocation<ProcessAction, MsgDataPacket.DataAttribute, ActionType>;
    public unsafe delegate void ProcessAction(Client.GameClient user, ServerSockets.Packet msg, ActionQuery* data);

    public class CustomCommands
    {
        public const ushort
            ExitQuestion = 1,
            Minimize = 2,
            ShowReviveButton = 1053,
            FlowerPointer = 1067,
            Enchant = 1091,
            LoginScreen = 1153,
            SelectRecipiet = 30,
            JoinGuild = 34,
            MakeFriend = 38,
            ChatWhisper = 40,
            CloseClient = 43,
            HotKey = 53,
            Furniture = 54,
            TQForum = 79,
            PathFind = 97,
            LockItem = 102,
            ShowRevive = 1053,
            HideRevive = 1054,
            StatueMaker = 1066,
            GambleOpen = 1077,
            GambleClose = 1078,
            Compose = 1086,
            Craft1 = 1088,
            Craft2 = 1089,
            Warehouse = 1090,
            ShoppingMallShow = 1100,
            ShoppingMallHide = 1101,
            NoOfflineTraining = 1117,
            Interact = 1122,
            CenterClient = 1155,
            ClaimCP = 1197,
            ClaimAmount = 1198,
            MerchantApply = 1201,
            MerchantDone = 1202,
            RedeemEquipment = 1233,
            ClaimPrize = 1234,
            RepairAll = 1239,
            FlowerIcon = 1244,
            SendFlower = 1246,
            ReciveFlower = 1248,
            WarehouseVIP = 1272,
            UseExpBall = 1288,
            HackProtection = 1298,
            HideGUI = 1307,
            Inscribe = 3059,
            BuyPrayStone = 3069,
            HonorStore = 3104,
            Opponent = 3107,
            CountDownQualifier = 3109,
            QualifierStart = 3111,
            ItemsReturnedShow = 3117,
            ItemsReturnedWindow = 3118,
            ItemsReturnedHide = 3119,
            QuestFinished = 3147,
            QuestPoint = 3148,
            QuestPointSparkle = 3164,
            StudyPointsUp = 3192,
            Updates = 3218,
            IncreaseLineage = 3227,
            HorseRacingStore = 3245,
            GuildPKTourny = 3249,
            QuitPK = 3251,
            Spectators = 3252,
            CardPlayOpen = 3270,
            CardPlayClost = 3271,
            ArtifactPurification = 3344,
            SafeguardConvoyShow = 3389,
            SafeguardConvoyHide = 3390,
            RefineryStabilization = 3392,
            ArtifactStabilization = 3398,
            SmallChat = 3406,
            NormalChat = 3407,
            Reincarnation = 3439;
    }
    public class DialogCommands
    {
        public const ushort
            Compose = 1,
            Craft = 2,
            Warehouse = 4,
            DetainRedeem = 336,
            DetainClaim = 337,
            VIPWarehouse = 341,
            Breeding = 368,
            PurificationWindow = 455,
            StabilizationRifinery = 448,
            StabilizationWindow = 459,
            TalismanUpgrade = 347,
            GemComposing = 422,
            OpenSockets = 425,
            Blessing = 426,
            TortoiseGemComposing = 438,
            RefineryStabilization = 448,
        
            HorseRacingStore = 464,
            Reincarnation = 485,
            ChangeName = 489,
            GarmentShop = 502,
            DegradeEquip = 506,
            Browse = 572,
            JiangHuSetName = 617,
            SendTwinCityTime = 538,
            BrowseAuction = 572,
            HowGetStudy = 595,
            TheFactionWar = 599, //-> 1072 packet
            ResetSecundaryPassword = 639,
            NewLottery = 656, // Packet -> 2804
            CreateUnion = 693,
            SetKingdomTitle = 700,
            ChangeNameUnion = 723;

    }
    public enum ActionType : ushort
    {
        RemoveTrap = 434,
        UpdateSpell = 252,
        UpdateProf = 253,
        DragonBall = 165,
        OpenGuiNpc = 160,
        AutoPatcher = 162,
        CountDown = 159,
        ChangeLookface = 151,
        SetLocation = 74,
        Hotkeys = 75,
        ConfirmAssociates = 76,
        ConfirmProficiencies = 77,
        ConfirmSpells = 78,
        ChangeDirection = 79,
        ChangeStance = 81,
        ChangeMap = 85,

        Teleport = 86,
        Leveled = 92,
        Revive = 94,
        DeleteCharacter = 95,
        SetPkMode = 96,
        ConfirmGuild = 97,
        LocationTeamLieder = 101,
        RequestEntity = 102,
        SetMapColor = 104,
        TeamSearchForMember = 106,
        RemoveSpell = 109,
        StartVendor = 111,
        StopVending = 114,
        OpenCustom = 116,
        ViewEquipment = 117,
        EndTransformation = 118,
        EndFly = 120,
        ViewEnemyInfo = 123,
        OpenDialog = 126,
        CompleteLogin = 132,
        ReviveMonster = 134,
        RemoveEntity = 135,
        Jump = 137,
        Ghost = 145,
        ViewFriendInfo = 148,
        ChangeFace = 151,
        TradePartnerInfo = 152,
        AbortMagic = 163,
        Bulletin = 166,
      
        PokerTeleporter = 167,
        FlashStep = 156,
        Away = 161,
        Pick = 164,
        ClikerON = 171,
        ClikerEntry = 172,
        SetAppearanceType = 178,
        PoketTableBet = 234,
        AllowAnimation = 251,
        CreditGifts = 255,
        UpdateInventorySash = 256,

        QuerySpawn = 310,
        QueryEquipment = 408,
        BeginSteedRace = 401,
        FinishSteedRace = 402,
        SubmitGoldBrick = 436,
        AddBlackList = 440,
        RemoveBlackList = 441,
        DrawStory = 443,
        PetAttack = 447
        /*Packet Nr 448. Server -> Client, Length : 55, PacketType: 10010
2F 00 1A 27 B4 E5 4F 03 22 4F 2E 00 00 00 00 00      ;/ '´åO"O.     
00 00 00 00 B4 E5 4F 03 CE 00 00 00 00 00 00 00      ;    ´åOÎ       
00 00 00 00 B4 E5 4F 03 00 01 00 00 00 00 00 54      ;    ´åO      T
51 53 65 72 76 65 72                                 ;QServer*/
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActionQuery
    {
        public int PacketStamp;
        public uint ObjId;

        public uint dwParam;//12
        public uint dwParam2;//16
        public ushort dwParam_Lo
        {
            get { return (ushort)dwParam; }
            set { dwParam = (uint)((dwParam_Hi << 16) | value); }
        }
        public ushort dwParam_Hi
        {
            get { return (ushort)(dwParam >> 16); }
            set { dwParam = (uint)((value << 16) | dwParam_Lo); }
        }
        public long PokerTableBet
        {
            get { fixed (void* ptr = &dwParam) { return *((long*)ptr); } }
            set { fixed (void* ptr = &dwParam) { *((long*)ptr) = value; } }
        }
        public long PokerRoundBet
        {
            get { fixed (void* ptr = &wParam1) { return *((long*)ptr); } }
            set { fixed (void* ptr = &wParam1) { *((long*)ptr) = value; } }
        }
        public int Timestamp;
        public ActionType Type;
        public ushort Fascing;//26
        public ushort wParam1;
        public ushort wParam2;
        public uint dwParam3;
        public uint dwParam4;
        public ushort wParam5;
    }
    public unsafe static class MsgDataPacket
    {
        /*2F 00 1A 27 6D B4 1D 00 15 2E 13 00 DC 00 00 00      ;/ 'm´ . Ü   
00 00 00 00 6D B4 1D 00 A4 00 01 00 00 00 00 00      ;    m´ ¤      
00 00 00 00 05 00 00 00 00 01 04 50 69 63 6B 54      ;        PickT
51 53 65 72 76 65 72                                 ;QServer*/
        /*2E 00 1A 27 B3 C5 1D 00 15 2E 13 00 00 00 00 00      ;. '³Å .     
00 00 00 00 00 00 00 00 8D 04 00 00 15 01 02 08      ;          
00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 51      ;              TQ
53 65 72 76 65 72                                    ;Server*/

        public static unsafe ServerSockets.Packet ActionPick(this ServerSockets.Packet stream, uint UID, ushort dwparam2, ushort timer, string text)
        {
            stream.InitWriter();
            stream.Write(Extensions.Time32.Now.GetHashCode());
            stream.Write(UID);
            stream.Write(220);
            stream.Write(0);
            stream.Write(Extensions.Time32.Now.GetHashCode());
            stream.Write((ushort)ActionType.Pick);
            stream.Write(dwparam2);
            stream.ZeroFill(8);
            stream.Write(timer);
            stream.ZeroFill(3);
            stream.Write(text);
            stream.Finalize(GamePackets.DataMap);
            return stream;
        }

        public static unsafe void Action(this ServerSockets.Packet stream, ActionQuery* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(ActionQuery));
        }
        public static unsafe ServerSockets.Packet ActionCreate(this ServerSockets.Packet stream, ActionQuery* pQuery)
        {
           // Console.WriteLine(Environment.TickCount + 3049841);
            //Console.WriteLine(pQuery->PacketStamp);
            if (pQuery->Type == ActionType.Jump)
                pQuery->PacketStamp = pQuery->Timestamp = Environment.TickCount;
           if (pQuery->Timestamp == 0)
                pQuery->Timestamp = pQuery->PacketStamp;
            stream.InitWriter();
            stream.WriteUnsafe(pQuery, sizeof(ActionQuery));
            stream.Finalize(GamePackets.DataMap);
            return stream;
        }
        public static unsafe ServerSockets.Packet ActionCreateWithString(this ServerSockets.Packet stream, ActionQuery* pQuery, params string[] str)
        {
            pQuery->PacketStamp = Extensions.Time32.Now.GetHashCode();
            if (pQuery->Timestamp == 0)
                pQuery->Timestamp = pQuery->PacketStamp;
            stream.InitWriter();
            stream.WriteUnsafe(pQuery, sizeof(ActionQuery));
            stream.SeekBackwards(1);
            stream.Write(str);
            stream.Finalize(GamePackets.DataMap);
            return stream;
        }
        public class DataAttribute : Attribute
        {
            public static readonly Func<DataAttribute, ActionType> Translator = (a) => a.Type;
            public ActionType Type { get; private set; }
            public DataAttribute(ActionType type)
            {
                this.Type = type;
            }
        }

        public static ActionInvoker invoker = new ActionInvoker(DataAttribute.Translator);
   
        [PacketAttribute(GamePackets.DataMap)]
        private unsafe static void Process(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
           
                ActionQuery data;
                stream.Action(&data);
              
                Tuple<DataAttribute, ProcessAction> processFolded;
                if (invoker.TryGetInvoker(data.Type, out processFolded))
                    processFolded.Item2(user, stream, &data);
                else
                {

#if TEST
                        if (user.ProjectManager)
                            MyConsole.WriteLine("DataMap not find  " + data.Type + " ");
#endif

                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        [DataAttribute(ActionType.SubmitGoldBrick)]
        public unsafe static void SubmitGoldBrick(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.InUnion)
            {
               // if(client.Player.UnionMemeber.
            }
        }
        [DataAttribute(ActionType.Bulletin)]
        public unsafe static void Bulletin(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                  || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
              || client.Player.Map == 1806
                  || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                  || client.Player.Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
          || client.Player.Map == 1768
          || client.Player.Map == Game.MsgTournaments.MsgFootball.MapID
          || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507)
            {
                return;
            }

            if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                || client.Player.Map == 6001)
            {

                return;
            }
            if (Program.BlockTeleportMap.Contains(client.Player.Map))
                return;
            switch (data->dwParam)
            {
                case 105://house
                    {
                        client.Teleport(200, 95, 1036);
                        client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
            }
        }
        [DataAttribute(ActionType.RemoveBlackList)]
        public unsafe static void RemoveBlackList(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            int size = msg.ReadInt8();
            string TargetName = msg.ReadCString(size);
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Name == TargetName)
                {
                    data->ObjId = user.Player.UID;
                    break;
                }
            }
            data->dwParam = 1;
            client.Send(msg.ActionCreateWithString(data, TargetName));
        }
        [DataAttribute(ActionType.AddBlackList)]
        public unsafe static void AddBlackList(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            int size = msg.ReadInt8();
            string TargetName = msg.ReadCString(size);
            foreach (var user in Database.Server.GamePoll.Values)
            {
                if (user.Player.Name == TargetName)
                {
                    data->ObjId = user.Player.UID;
                    break;
                }
            }
            data->dwParam = 1;
            client.Send(msg.ActionCreateWithString(data, TargetName));
           
        }

        [DataAttribute(ActionType.PetAttack)]
        public unsafe static void PetAttack(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {




            foreach (var obj in client.Player.View.Roles(Role.MapObjectType.Monster))
            {
                var pet = obj as Game.MsgMonster.MonsterRole;
                if (pet.IsFloor)
                {
                    if (Extensions.Time32.Now > pet.AttackSpeed)
                    {
                        pet.AttackSpeed = Extensions.Time32.Now.AddMilliseconds(500);
                        uint Experience = 0;
                        Role.IMapObj target;
                        if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Monster))
                        {


                            var DBSpell = Database.Server.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackMonster.Verified(client, attacked, DBSpell))
                            {



                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                             , 0, attacked.X, attacked.Y, DBSpell.ID
                                             , DBSpell.Level, 0);


                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Range.OnMonster(client.Player, attacked, DBSpell, out AnimationObj
                                    , (byte)(pet.ContainFlag(MsgUpdate.Flags.Thunderbolt) ? 2 : 0));
                                AnimationObj.Damage = MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                Experience += MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(msg, AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                            }

                        }
                        if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Player))
                        {
                            var DBSpell = Database.Server.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            var attacked = target as Role.Player;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackPlayer.Verified(client, attacked, DBSpell))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                            , 0, attacked.X, attacked.Y, DBSpell.ID
                                            , DBSpell.Level, 0);
                                int extra = DBSpell.Damage2;
                                MsgSpell myspell;
                                if (pet.ContainFlag(MsgUpdate.Flags.Thunderbolt) && client.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.Thunderbolt, out myspell))
                                    extra = 10 * (myspell.Level + 1);

                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Range.OnPlayer(client.Player, attacked, DBSpell, out AnimationObj, (int)(extra));
                                AnimationObj.Damage = MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                            }
                        }
                        if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.SobNpc))
                        {
                            var DBSpell = Database.Server.Magic[(ushort)Role.Flags.SpellID.ThundercloudAttack][0];
                            var attacked = target as Role.SobNpc;
                            if (MsgServer.AttackHandler.CheckAttack.CanAttackNpc.Verified(client, attacked, DBSpell))
                            {
                                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(pet.UID
                                         , 0, attacked.X, attacked.Y, DBSpell.ID
                                         , DBSpell.Level, 0);

                                MsgSpellAnimation.SpellObj AnimationObj;
                                MsgServer.AttackHandler.Calculate.Range.OnNpcs(client.Player, attacked, DBSpell, out AnimationObj);
                                AnimationObj.Damage = MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);
                                Experience += MsgServer.AttackHandler.ReceiveAttack.Npc.Execute(msg, AnimationObj, client, attacked);
                                MsgSpell.Targets.Enqueue(AnimationObj);
                                MsgSpell.SetStream(msg);
                                MsgSpell.Send(client);
                            }
                        }
                        MsgServer.AttackHandler.Updates.IncreaseExperience.Up(msg, client, Experience);
                    }
                }
            }
        }
       
        [DataAttribute(ActionType.ClikerEntry)]
        public unsafe static void ClikerEntry(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            //dwparam target.
            if (client.Player.QuestGUI.CheckQuest(6131, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Role.IMapObj target;
                if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Monster))
                {
                    var monster = target as Game.MsgMonster.MonsterRole;

                    if (monster.Name.Contains("BanditL98"))
                    {
                        client.Player.QuestGUI.IncreaseQuestObjectives(msg, 6131, 0,1);
                        client.Player.QuestGUI.IncreaseQuestObjectives(msg, 6131, 1, 1);
                    }

                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);
                }
                data->ObjId = data->dwParam;
                data->dwParam = 0;
                data->Type = (ActionType)174;
                client.Send(msg.ActionCreate(data));
            }
        else    if (client.Player.QuestGUI.CheckQuest(1355, MsgQuestList.QuestListItem.QuestStatus.Accepted))
            {
                Role.IMapObj target;
                if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Monster))
                {
                    var monster = target as Game.MsgMonster.MonsterRole;
#if Arabic
                     if (monster.Name.Contains("Thunder"))
                    {
                        client.SendSysMesage("Now you can report back to Poison Master.", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1355, 1);
                    }
#else
                    if (monster.Name.Contains("Thunder"))
                    {
                        client.SendSysMesage("Now you can report back to Poison Master.", MsgMessage.ChatMode.System);
                        client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1355, 1);
                    }
#endif
                   
                    monster.HitPoints = 0;
                    monster.Dead(msg, client, client.Player.UID, client.Map);
                }
                data->ObjId = data->dwParam;
                data->dwParam = 0;
                data->Type = (ActionType)174;
                client.Send(msg.ActionCreate(data));
            }
            else  if (client.Player.QuestGUI.CheckQuest(1487, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    Role.IMapObj target;
                    if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Monster))
                    {
                        var monster = target as Game.MsgMonster.MonsterRole;

                        if (monster.Name == "RockMonsterL78")
                        {
#if Arabic
                            client.CreateBoxDialog("The~Level~78~Rock~Monsters~are~afraid~of~the~Cactus~and~run~away.~Hurry~to~report~to~Convoy~Vice~Leader~Ling.");
#else
                             client.CreateBoxDialog("The~Level~78~Rock~Monsters~are~afraid~of~the~Cactus~and~run~away.~Hurry~to~report~to~Convoy~Vice~Leader~Ling.");
#endif
                            monster.HitPoints = 0;
                            monster.Dead(msg, client, client.Player.UID, client.Map);
                            client.Player.QuestGUI.IncreaseQuestObjectives(msg, 1487, 1);

                        }
                    }
                }
                else if (client.Player.QuestGUI.CheckQuest(1330, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                {
                    Role.IMapObj target;
                    if (client.Player.View.TryGetValue(data->dwParam, out target, Role.MapObjectType.Monster))
                    {
                        var monster = target as Game.MsgMonster.MonsterRole;
#if Arabic
                     if (monster.Name == "ThunderApe")
                    {
                        if (!client.Player.ActivePick)
                        {
                            client.Player.QuestCaptureType = 1;
                            client.Player.AddPick(msg, "Capture~Thunder~Ape.", 2);
                        }
                    }
                    else if (monster.Name == "ThunderApeL58")
                    {
                        if (!client.Player.ActivePick)
                        {
                            client.Player.QuestCaptureType = 2;
                            client.Player.AddPick(msg, "Capture~Thunder~ApeL58.", 2);
                        }
                    }
#else
                        if (monster.Name == "ThunderApe")
                        {
                            if (!client.Player.ActivePick)
                            {
                                client.Player.QuestCaptureType = 1;
                                client.Player.AddPick(msg, "Capture~Thunder~Ape.", 2);
                            }
                        }
                        else if (monster.Name == "ThunderApeL58")
                        {
                            if (!client.Player.ActivePick)
                            {
                                client.Player.QuestCaptureType = 2;
                                client.Player.AddPick(msg, "Capture~Thunder~ApeL58.", 2);
                            }
                        }
#endif

                        monster.HitPoints = 0;
                        monster.Dead(msg, client, client.Player.UID, client.Map);
                    }
                    data->ObjId = data->dwParam;
                    data->dwParam = 0;
                    data->Type = (ActionType)174;
                    client.Send(msg.ActionCreate(data));
                }
        }
        [DataAttribute(ActionType.PokerTeleporter)]
        public unsafe static void PokerTeleporter(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Map == 1017 || client.Player.Map == 1081 || client.Player.Map == 2060 || client.Player.Map == 9972
                     || client.Player.Map == 1080 || client.Player.Map == 3820 || client.Player.Map == 3954
                 || client.Player.Map == 1806
                     || Game.MsgTournaments.MsgSchedules.DisCity.IsInDisCity(client.Player.Map) || client.Player.Map == 1508
                     || client.Player.Map == Game.MsgTournaments.MsgTeamDeathMatch.MapID || Game.MsgTournaments.MsgSchedules.SteedRace.InSteedRace(client.Player.Map)
             || client.Player.Map == 1768
             || client.Player.Map == Game.MsgTournaments.MsgFootball.MapID
             || client.Player.Map == 1505 || client.Player.Map == 1506 || client.Player.Map == 1509 || client.Player.Map == 1508 || client.Player.Map == 1507)
            {
#if Arabic
                                       client.SendSysMesage("The Poker room is not allowed on this map.");
#else
                client.SendSysMesage("The Poker room is not allowed on this map.");
#endif

                return;
            }

            if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID || client.Player.DynamicID != 0
                || client.Player.Map == 6001)
            {
#if Arabic
                   client.SendSysMesage("The Poker room is not allowed on this map.");
#else
                client.SendSysMesage("The Poker room is not allowed on this map.");
#endif

                return;
            }
            if (Program.BlockTeleportMap.Contains(client.Player.Map))
                return;
            switch (data->dwParam)
            {
                case 4://roulete
                    {
#if Roullete
                        client.Teleport(56, 65, 3852);
#endif
                        break;
                    }
#if Poker
                case 2://cp room
                    {
                        client.Teleport(58, 66, 1860);
                        break;
                    }
                case 1://gold room
                    {
                        client.Teleport(157, 119, 1858);
                        break;
                    }
#endif
                case 3://one bandit
                    {
                        client.Teleport(240, 241, 1036);
                        break;
                    }
            }

        }
        [DataAttribute(ActionType.CreditGifts)]
        public unsafe static void CreditGifts(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if ((client.Player.MainFlag & Role.Player.MainFlagType.CanClaim) ==  Role.Player.MainFlagType.CanClaim)
            {
                if ((client.Player.MainFlag & Role.Player.MainFlagType.ClaimGift) != Role.Player.MainFlagType.ClaimGift)
                {
                    if (client.Inventory.HaveSpace(9))
                    {
                        client.Inventory.Add(msg, 184305, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 1100003, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 3000782, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 3000781, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 3001266, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 3001289, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 3001036, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);
                        client.Inventory.Add(msg, 1200002, 1, 0, 0, 0, Role.Flags.Gem.NoSocket, Role.Flags.Gem.NoSocket, true);


                      

                        client.Player.MainFlag |= Role.Player.MainFlagType.ClaimGift;
                        client.Player.MainFlag |= Role.Player.MainFlagType.ShowSpecialItems;
                        client.Player.SendUpdate(msg, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag);
                   
                        client.Player.SendUpdate(msg, (uint)client.Player.MainFlag, MsgUpdate.DataType.MainFlag);



                        Console.WriteLine((uint)client.Player.MainFlag);
                     
                    }
                }
            }
        }
        [DataAttribute(ActionType.UpdateInventorySash)]
        public unsafe static void UpdateInventorySash(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.ConquerPoints >= 90 && client.Player.InventorySashCount < 300)
            {
                client.Player.InventorySashCount += 1;
                client.Player.ConquerPoints -= 90;
                client.Player.UpdateInventorySash(msg);
            }
            else
            {
#if Arabic
                 client.SendSysMesage("You need 90 CPs in your inventory to open 1 slot in your sash.");
#else
                client.SendSysMesage("You need 90 CPs in your inventory to open 1 slot in your sash.");
#endif
               
            }
        }
        [DataAttribute(ActionType.Away)]
        public unsafe static void Away(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (data->ObjId == client.Player.UID)
            {
                if (client.Player.Away == 1)
                    client.Player.Away = 0;
                else
                    client.Player.Away = 1;
            }
            else
                client.Player.Away = 0;

            client.Send(msg.ActionCreate(data));
            client.Player.View.SendView(client.Player.GetArray(msg, false), false);
        }
        [DataAttribute(ActionType.TradePartnerInfo)]
        public unsafe static void TradePartnerInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Client.GameClient Target;
            if (Database.Server.GamePoll.TryGetValue(data->dwParam, out Target))
            {
                client.Send(msg.TradePartnerInfoCreate(Target));
            }
        }
        [DataAttribute(ActionType.SetAppearanceType)]
        public unsafe static void SetAppearanceType(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Player.AparenceType = data->dwParam;
            data->ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ViewEnemyInfo)]
        public unsafe static void ViewEnemyInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Client.GameClient Target;
            if (Database.Server.GamePoll.TryGetValue(data->dwParam, out Target))
            {
                client.Send(msg.KnownPersonInfoCreate(Target, true));
            }
        }
        [DataAttribute(ActionType.FinishSteedRace)]
        public unsafe static void FinishSteedRace(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Map == MsgTournaments.MsgSteedRace.MAPID)
                MsgTournaments.MsgSchedules.SteedRace.FinishRace(client);
        }
        [DataAttribute(ActionType.ViewFriendInfo)]
        public unsafe static void ViewFriendInfo(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Client.GameClient Target;
            if (Database.Server.GamePoll.TryGetValue(data->dwParam, out Target))
            {
                client.Send(msg.KnownPersonInfoCreate(Target, false));
            }
        }
        [DataAttribute(ActionType.Ghost)]
        public unsafe static void Ghost(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Alive == false)
            {
                client.Player.SendString(msg, MsgStringPacket.StringID.Effect, true, "ghost");
                data->ObjId = client.Player.UID;
                data->wParam1 = client.Player.X;
                data->wParam2 = client.Player.Y;
                client.Player.View.SendView(msg.ActionCreate(data), true);
            }   
        }
        [DataAttribute(ActionType.StartVendor)]
        public unsafe static void StartVendor(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (!client.IsVendor)
            {
                client.MyVendor = new Role.Instance.Vendor(client);
                client.MyVendor.CreateVendor(msg);
                data->dwParam = client.MyVendor.VendorUID;
                data->wParam1 = client.MyVendor.VendorNpc.X;
                data->wParam2 = client.MyVendor.VendorNpc.Y;
                client.Send(msg.ActionCreate(data));
            }
        }
        [DataAttribute(ActionType.Revive)]
        public unsafe static void Revive(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.ContainFlag(MsgUpdate.Flags.SoulShackle))
                return;
            if (Extensions.Time32.Now > client.Player.DeadStamp.AddSeconds(20))
            {
                if (!client.Player.Alive)
                {
                    bool ReviveHere = data->dwParam == 1;
                    if (ReviveHere)
                    {
                        if (client.Player.Map == 1038 || client.Player.Map == MsgTournaments.MsgClassPKWar.MapID
                            || client.Player.Map == MsgTournaments.MsgSuperGuildWar.MapID
                            || client.Player.Map == MsgTournaments.MsgCaptureTheFlag.MapID
                            || client.Player.Map == MsgTournaments.MsgTeamDeathMatch.MapID)
                            return;
                        client.Player.Revive(msg);
                        return;
                    }
                    else
                    {
                        if (client.Player.Map == 4000 || client.Player.Map == 4003 || client.Player.Map == 4006 || client.Player.Map == 4008 || client.Player.Map == 4009)
                        {
                            client.Teleport(85, 75, 4020);
                            return;
                        }
                        if (client.Player.Map == 3998)
                        {
                            client.Teleport(106, 383, 3998);
                            return;
                        }
                        if (client.Player.Map == 3935 && client.OnInterServer)
                        {
                            foreach (var server in Database.GroupServerList.GroupServers.Values)
                            {
                                if (server.ID == client.Player.ServerID)
                                {
                                    client.Teleport((ushort)server.X, (ushort)server.Y, (ushort)server.MapID); 
                                    break;
                                }
                            }
                            return;
                        }
                        if (client.Player.OnMyOwnServer == false)
                        {
                            client.Teleport((ushort)432, 390, 1002);
                            return;
                        }
                        if (Game.MsgTournaments.MsgSchedules.ClanWar.Process == MsgTournaments.ProcesType.Alive)
                        {
                            if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar != null)
                            {
                                if (Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.InWar(client))
                                {
                                    Game.MsgTournaments.MsgSchedules.ClanWar.CurentWar.Teleport(client);
                                    return;
                                }
                            }
                        }
                        if (MsgTournaments.MsgSchedules.CurrentTournament.Type == MsgTournaments.TournamentType.TeamDeathMatch)
                        {
                            if (MsgTournaments.MsgSchedules.CurrentTournament.InTournament(client))
                            {
                                var tournament = (MsgTournaments.MsgSchedules.CurrentTournament as MsgTournaments.MsgTeamDeathMatch);
                                tournament.TeleportPlayer(tournament.GetTeamTyp(client.Player.GarmentId), client);
                                return;
                            }
                        }
                       
                        if (client.Player.Map == 1762 || client.Player.Map == 1927 || client.Player.Map == 1999 || client.Player.Map == 2054 || client.Player.Map == 2055)
                        {
                            client.Teleport(477, 640, 1000);
                            return;
                        }
                        if (client.Player.Map == 1038)// gw map
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(301, 278, 1002, 0, true, true);
                            else
                            {
                                var map = Database.Server.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map,0,true, true);
                            }
                        }
                        else if (client.Player.Map == 3868)//super gw
                        {
                            if (MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(301, 278, 1002);
                            else
                            {
                                var map = Database.Server.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else
                        {
                            if (client.Map.Reborn_Map != client.Player.Map)
                            {
                                if (client.Map.Reborn_Map == 0)
                                {
                                    client.Teleport(301, 278, 1002);
                                    return;
                                }
                                var map = Database.Server.ServerMaps[client.Map.Reborn_Map];
                                client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                            }
                            else
                            {
                                if (client.Map.Reborn_X != 0)
                                    client.Teleport(client.Map.Reborn_X, client.Map.Reborn_Y, client.Map.Reborn_Map);
                                else
                                {
                                    var map = Database.Server.ServerMaps[client.Map.Reborn_Map];
                                    if (map.Reborn_X != 0)
                                        client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                    else
                                    {
                                        map = Database.Server.ServerMaps[map.Reborn_Map];
                                        if (map.Reborn_X != 0)
                                            client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                        else
                                            client.Teleport(301, 278, 1002);
                                    }
                                }
                            }
                        }
                        if (client.Player.X == 0 || client.Player.Y == 0)//invalid map reborn
                            client.Teleport(301, 278, 1002);
                    }
                }
            }
        }
        [DataAttribute(ActionType.EndTransformation)]
        public unsafe static void EndTransformation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.OnTransform)
            {
                if (client.Player.TransformInfo != null)
                {
                    client.Player.TransformInfo.Stamp = Extensions.Time32.Now;
                }
            }
        }
        [DataAttribute(ActionType.UpdateSpell)]
        public unsafe static void UpdateSpell(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            MsgSpell Spell;
            if (client.MySpells.ClientSpells.TryGetValue((ushort)data->dwParam, out Spell))
            {
                Dictionary<ushort, Database.MagicType.Magic> DbSpells;
                if (Database.Server.Magic.TryGetValue(Spell.ID, out DbSpells))
                {
                    if (Spell.Level < DbSpells.Count - 1)
                    {

                        decimal cpCost = DbSpells[Spell.Level].CpsCost;
                        cpCost = ((cpCost / 2) - (cpCost / 2 / 10)) / 10;

                        int max = Math.Max((int)Spell.Experience, 1);
                        int percentage = (int)Math.Ceiling((decimal)(100 - (int)(max / Math.Max((DbSpells[Spell.Level].Experience / 100), 1))));
                        cpCost = Math.Ceiling((decimal)(cpCost * percentage / 100));

                        if (client.Player.ConquerPoints >= cpCost)
                        {
                            client.Player.ConquerPoints -= (uint)cpCost;

                            client.MySpells.Add(msg, Spell.ID, (ushort)(Spell.Level + 1), Spell.SoulLevel, Spell.PreviousLevel, Spell.Experience);
                        }
                    }
                }
            }
        }
        [DataAttribute(ActionType.UpdateProf)]
        public unsafe static void UpdateProf(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            MsgProficiency prof;
            if (client.MyProfs.ClientProf.TryGetValue(data->dwParam, out prof))
            {
                if (prof.Level < 20)
                {
                    ushort PriceUpdate = Database.Server.PriceUpdatePorf[prof.Level];
                    uint NeedExperience = Role.Instance.Proficiency.MaxExperience - prof.Experience;
                    uint Cost = (uint)Math.Ceiling((double)(NeedExperience / PriceUpdate));
                    if (client.Player.ConquerPoints >= Cost)
                    {
                        client.Player.ConquerPoints -= Cost;

                        client.MyProfs.Add(msg, prof.ID, prof.Level + 1, prof.Experience, prof.PreviouseLevel);
                    }
                }
            }
        }
        [DataAttribute(ActionType.StopVending)]
        public unsafe static void StopVending(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.IsVendor)
                client.MyVendor.StopVending(msg);
        }
        [DataAttribute(ActionType.EndFly)]
        public unsafe static void EndFly(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Player.RemoveFlag(MsgUpdate.Flags.Fly);
        }
        [DataAttribute(ActionType.TeamSearchForMember)]
        public unsafe static void TeamSearchForMember(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Team != null)
            {
                Client.GameClient Member;
                if (client.Team.TryGetMember(data->dwParam, out Member))
                {
                    if (Member.Player.Map == client.Player.Map)
                    {
                        data->wParam1 = Member.Player.X;
                        data->wParam2 = Member.Player.Y;
                        client.Send(msg.ActionCreate(data));
                    }
                }
            }
        }
        [DataAttribute(ActionType.SetLocation)]
        public unsafe static void SetLocation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if ((client.ClientFlag & Client.ServerFlag.SetLocation) != Client.ServerFlag.SetLocation)
            {


#if !Roullete
                if (client.Player.Map == 3852)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 301;
                    client.Player.Y = 278;
                }
#endif
    
#if !Poker
                if (client.Player.Map == 1860 || client.Player.Map == 1858)
                {
                    client.Player.Map = 1002;
                    client.Player.X = 301;
                    client.Player.Y = 278;
                }
#endif
                if (!Role.GameMap.CheckMap(client.Player.Map))
                {
                    client.Player.Map = 1002;
                    client.Player.X = 301;
                    client.Player.Y = 278;
                }
            back:
                if (Database.Server.ServerMaps.TryGetValue(client.Player.Map, out client.Map))
                {

                    client.ClientFlag |= Client.ServerFlag.SetLocation;
                    client.Map.Enquer(client);

                    if (client.Player.HitPoints == 0)
                    {
                        client.Player.HitPoints = 1;

                        if (client.Player.Map == 1038)// gw map
                        {
                            if (MsgTournaments.MsgSchedules.GuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(301, 278, 1002);
                            else
                            {
                                var map = Database.Server.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else if (client.Player.Map == 3868)//super gw
                        {
                            if (MsgTournaments.MsgSchedules.SuperGuildWar.Proces == MsgTournaments.ProcesType.Dead)
                                client.Teleport(301, 278, 1002);
                            else
                            {
                                var map = Database.Server.ServerMaps[1038];
                                client.Teleport(27, 73, map.Reborn_Map);
                            }
                        }
                        else
                        {
                            if (client.Player.Map == 601)
                                client.Teleport(301, 278, 1002);
                            else
                            {
                                if (client.Map.Reborn_X != 0)
                                    client.Teleport(client.Map.Reborn_X, client.Map.Reborn_Y, client.Map.Reborn_Map);
                                else
                                {
                                    Role.GameMap map;// ;= Database.Server.ServerMaps[client.Map.Reborn_Map];
                                    if (Database.Server.ServerMaps.TryGetValue(client.Map.Reborn_Map, out map))
                                    {
                                        if (map.Reborn_X != 0)
                                            client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                        else
                                        {
                                            map = Database.Server.ServerMaps[map.Reborn_Map];
                                            if (map.Reborn_X != 0)
                                                client.Teleport(map.Reborn_X, map.Reborn_Y, map.Reborn_Map);
                                            else
                                                client.Teleport(301, 278, 1002);
                                        }
                                    }
                                    else
                                    {
                                        
                                        client.Teleport(301, 278, 1002);
                                    }
                                }
                                if (client.Player.X == 0 || client.Player.Y == 0)//invalid map reborn
                                    client.Teleport(301, 278, 1002);
                            }
                        }
                    }

                    if (client.Map.BaseID != 0)
                        data->dwParam = client.Map.BaseID;
                    else
                        data->dwParam = client.Player.Map;
                    data->wParam1 = client.Player.X;
                    data->wParam2 = client.Player.Y;
                    client.Send(msg.ActionCreate(data));




                    client.Player.ClearPreviouseCoord();
                    client.Player.View.Role();



                }
                else
                {
                    client.Player.Map = 1002;
                    client.Player.X = 301;
                    client.Player.Y = 278;
                    Database.Server.ServerMaps[1002].Enquer(client);
                    goto back;
                }

                client.Send(msg.MapStatusCreate(client.Map.ID, client.Map.ID, client.Map.TypeStatus));

                //client.Send(msg.WeatherCreate(MsgWeather.WeatherType.Snow, 1000, 3, 0, 0));
                if (client.Player.Map == 3846)
                {

                    ActionQuery datam = new ActionQuery()
                    {
                        ObjId = client.Player.UID,
                        Type = ActionType.SetMapColor,
                        dwParam = 16755370,
                        wParam1 = client.Player.X,
                        wParam2 = client.Player.Y
                    };

                    client.Send(msg.ActionCreate(&datam));
                }
            }

            /*2A 00 1A 27 A1 E3 4F 03 22 4F 2E 00 02 00 00 00      ;* '¡ãO"O.    
00 00 00 00 00 00 00 00 9D 00 00 00 91 00 5F 01      ;            _
00 00 00 00 00 00 00 00 00 00 54 51 53 65 72 76      ;          TQServ
65 72                                                ;er*/
            msg.InitWriter();
            msg.Write(Extensions.Time32.Now.Value);
            msg.Write(client.Player.UID);
            msg.Write((uint)0);
            msg.ZeroFill(8);
            msg.Write((uint)0x9d);
            msg.Write((ushort)91);
            msg.Write((byte)0x5f);
            msg.Write((byte)0x01);
            msg.ZeroFill(10
                );
            msg.Finalize(10010);
            client.Send(msg);


        }
        [DataAttribute(ActionType.ChangeMap)]
        public unsafe static void ChangeMap(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            ushort Portal_X = data->dwParam_Lo;
            ushort Portal_Y = data->dwParam_Hi;
            foreach (var portal in client.Map.Portals)
            {
                if (Role.Core.GetDistance(client.Player.X, client.Player.Y, portal.X, portal.Y) < 7)
                {
                    
                    client.Teleport(portal.Destiantion_X, portal.Destiantion_Y, portal.Destiantion_MapID);
                    client.Send(msg.ActionCreate(data));
                    return;
                }
            }
            if (client.ProjectManager)
            {
                client.SendSysMesage("Invalid Portal : X = " + Portal_X + ", Y= " + Portal_Y + " Map = " + client.Player.Map + "", MsgMessage.ChatMode.System, MsgMessage.MsgColor.yellow);
            }
            client.Teleport(310, 288, 1002);
        }
        [DataAttribute(ActionType.ChangeLookface)]
        public unsafe static void ChangeLookface(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Money >= 500)
            {
                uint newface = data->dwParam;
                if (client.Player.Body > 2000)
                {
                    newface = newface < 200 ? newface + 200 : newface;
                    client.Player.Face = (ushort)newface;
                }
                else
                {
                    newface = newface > 200 ? newface - 200 : newface;
                    client.Player.Face = (ushort)newface;
                }
                client.Player.Money -= 500;
                client.Send(msg.ActionCreate(data));
                client.Player.SendUpdate(msg, client.Player.Money, MsgUpdate.DataType.Money);
            }
            else
            {
#if Arabic
                client.SendSysMesage("You do not have 500 silvers with you.", MsgMessage.ChatMode.TopLeft);
#else
                client.SendSysMesage("You do not have 500 silvers with you.", MsgMessage.ChatMode.TopLeft);
#endif
                
            }
        }
        [DataAttribute(ActionType.RequestEntity)]
        public unsafe static void RequestEntity(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Role.IMapObj obj;
            if (client.Player.View.TryGetValue(data->dwParam, out obj, Role.MapObjectType.Player))
            {
                var pClient = (obj as Role.Player).Owner;
                if (pClient.IsWatching() || pClient.Player.Invisible)
                    return;
                client.Send(obj.GetArray(msg, false));
            }
            else if (client.Player.View.TryGetValue(data->dwParam, out obj, Role.MapObjectType.Monster))
            {
                client.Send(obj.GetArray(msg, false));
            }
        }

        [DataAttribute(ActionType.QuerySpawn)]
        public unsafe static void QuerySpawn(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Client.GameClient Target;
            if (Database.Server.GamePoll.TryGetValue(data->dwParam, out Target))
            {
               
                client.Send(Target.Player.GetArray(msg, true));
            }
           
        }
        [DataAttribute(ActionType.ViewEquipment)]
        public unsafe static void ViewEquipment(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            Client.GameClient Target;
            if (Database.Server.GamePoll.TryGetValue(data->dwParam, out Target))
            {
                if (Target.Equipment == null || Target.Equipment.CurentEquip == null) return;

                foreach (var item in Target.Equipment.CurentEquip)
                {
                    if (item != null)
                    {
                        client.Send(msg.ItemViewCreate(Target.Player.UID, 0, item, MsgItemView.ActionMode.ViewEquip));
                        item.SendItemExtra(client, msg);
                    }
                }
                MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = Target.Player.Level;
                info.UID = Target.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[19];
                for (int x = 0; x < 19; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = Target.PrestigePoints[x];
                }
                client.Send(msg.UserAbilityScoreCreate(info));
                if (!client.ProjectManager)
                {
#if Arabic
                 Target.SendSysMesage("" + client.Player.Name + " is observing your equipment !", MsgMessage.ChatMode.System);
#else
                    Target.SendSysMesage("" + client.Player.Name + " is observing your equipment !", MsgMessage.ChatMode.System);
#endif
                }
            }
        }
        [DataAttribute(ActionType.Hotkeys)]
        public unsafe static void Hotkeys(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Send(msg.ActionCreate(data));

        }
        public static int CalculateJumpStamp(short distance)
        {
            return (int)400 + distance * 40;
        }
        [DataAttribute(ActionType.Jump)]
        public unsafe static void PlayerJump(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.Away == 1)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var apacket = rec.GetStream();
                    client.Player.Away = 0;
                    client.Player.View.SendView(client.Player.GetArray(apacket, false), false);
                }
            }



            client.OnAutoAttack = false;
            client.Player.RemoveBuffersMovements(msg);

            if (!client.Player.Alive)
            {
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, client.Player.Dead_X, client.Player.Dead_Y);
                if (client.Player.Dead_X == 0 || client.Player.Dead_Y == 0)
                {
                    client.Player.X = 301;
                    client.Player.Y = 278;
                }
                else
                {
                    client.Player.X = client.Player.Dead_X;
                    client.Player.Y = client.Player.Dead_Y;
                }

                InteractQuery action = new InteractQuery()
                {
                    X = client.Player.Dead_X,
                    AtkType = MsgAttackPacket.AttackID.Death,
                    Y = client.Player.Dead_Y,
                    OpponentUID = client.Player.UID
                };
                client.Player.View.SendView(msg.InteractionCreate(&action), true);

                return;
            }

            ushort JumpX = data->dwParam_Lo;
            ushort JumpY = data->dwParam_Hi;

            client.Player.Protect = Extensions.Time32.Now;

            if (client.Map == null)
            {
                client.Teleport(310, 288, 1002);
                return;
            }
            if (client.Player.Map == 1038)
            {
                if (!Game.MsgTournaments.MsgSchedules.GuildWar.ValidJump(client.TerainMask, out client.TerainMask, JumpX, JumpY))
                {
#if Arabic
                     client.SendSysMesage("Illegal jumping over the gates detected.");
#else
                    client.SendSysMesage("Illegal jumping over the gates detected.");
#endif
                   
                    client.Pullback();
                    return;
                }
            }
            /*if (!client.Map.ValidLocation(JumpX, JumpY))
            {
                client.Teleport(client.Player.Px, client.Player.Py, client.Player.Map, client.Player.DynamicID);
                return;
            }*/
            if (client.Player.ObjInteraction != null)
            {

                InterActionWalk inter = new InterActionWalk()
             {
                 Mode = MsgInterAction.Action.Jump,
                 X = JumpX,
                 Y = JumpY,
                 UID = client.Player.UID,
                 OponentUID = client.Player.ObjInteraction.Player.UID
             };
                client.Player.View.SendView(msg.InterActionWalk(&inter), true);


                client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
                client.Player.Action = Role.Flags.ConquerAction.Jump;
                client.Map.View.MoveTo<Role.IMapObj>(client.Player, JumpX, JumpY);
                client.Player.X = JumpX;
                client.Player.Y = JumpY;
                client.Player.View.Role(false, null);//msg.ActionCreate(data));

                client.Map.View.MoveTo<Role.IMapObj>(client.Player.ObjInteraction.Player, JumpX, JumpY);
                client.Player.ObjInteraction.Player.X = JumpX;
                client.Player.ObjInteraction.Player.Y = JumpY;
                client.Player.ObjInteraction.Player.Action = Role.Flags.ConquerAction.Jump;
                client.Player.ObjInteraction.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
                client.Player.ObjInteraction.Player.View.Role(false, null);

                return;
            }
#if PROTECTION_SPEEDHACK
            if ((byte)(data->wParam5) != 1)
                client.BackJump = false;
            else
                client.BackJump = true;
            Extensions.Time32 ReceiveTime = new Extensions.Time32(data->Timestamp);
            if (client.LastClientJump >= ReceiveTime)
            {
#if Arabic
                client.SendSysMesage("You've been suspected of speed hacking.");
#else
                client.SendSysMesage("You've been suspected of speed hacking.");
#endif
                
                return;
            }
            if (!client.Player.ContainFlag(MsgUpdate.Flags.Cyclone) && !client.Player.ContainFlag(MsgUpdate.Flags.SuperCyclone) && !client.Player.ContainFlag(MsgUpdate.Flags.DragonCyclone)
                 && !client.Player.ContainFlag(MsgUpdate.Flags.Oblivion) && !client.Player.ContainFlag(MsgUpdate.Flags.BladeFlurry) && !client.Player.OnTransform)
            {
                if ((byte)(data->wParam5) != 1 && !client.BackJump)
                {
                    //Console.WriteLine(ReceiveTime.Value - client.LastClientJump.Value);
                    if (ReceiveTime.Value - client.LastClientJump.Value <= 500)
                    {
                        client.Player.CountSpeedHack++;
                        if (client.Player.CountSpeedHack == 3)
                        {

                            Console.WriteLine(client.Player.Name + "been suspected of speed hacking.");
                            // client.SendSysMesage("You've been suspected of speed hacking.");
                            client.Socket.Disconnect();
                            return;
                        }
                    }
                    /*  else if (Extensions.Time32.Now.Value - client.LastServerJump.Value <= 300)
                      {
                          client.Player.CountSpeedHack++;
                          if (client.Player.CountSpeedHack == 2)
                          {
                              Console.WriteLine(client.Player.Name + "been suspected of speed hacking.");
                              //client.SendSysMesage("You've been suspected of speed hacking.");
                              client.Socket.Disconnect();
                              return;
                          }
                      }
                      else*/
                    client.Player.CountSpeedHack = 0;
                }
                else if ((byte)(data->wParam5) != 1)
                {
                    client.BackJump = false;
                }
            }
            client.LastClientJump = ReceiveTime;
            client.LastServerJump = Extensions.Time32.Now;
#endif

            short Distance = Role.Core.GetDistance(client.Player.X, client.Player.Y, JumpX, JumpY);
           /* if (Distance > 18 && client.Player.Map != 2068)
            {
                client.Socket.Disconnect();
                return;
            }
         */
         
            client.Player.StampJump = DateTime.Now;
            client.Player.StampJumpMilisecounds = CalculateJumpStamp(Distance);
            if (client.Player.ContainFlag(MsgUpdate.Flags.Ride))
            {
                uint vigor = (ushort)(1.5F * (Distance / 2));
                if (client.Vigor >= vigor)
                    client.Vigor -= vigor;
                else
                    client.Vigor = 0;


                client.Send(msg.ServerInfoCreate(MsgServerInfo.Action.Vigor, client.Vigor));

            }
            data->dwParam3 = client.Player.Map;
            client.Player.View.SendView(msg.ActionCreate(data), true);
            client.Player.Angle = Role.Core.GetAngle(client.Player.X, client.Player.Y, JumpX, JumpY);
            client.Player.Action = Role.Flags.ConquerAction.Jump;
            client.Map.View.MoveTo<Role.IMapObj>(client.Player, JumpX, JumpY);
            client.Player.X = JumpX;
            client.Player.Y = JumpY;
            client.Player.View.Role(false, null);//msg.ActionCreate(data));
           
            if (MsgTournaments.MsgSchedules.CaptureTheFlag != null)
                MsgTournaments.MsgSchedules.CaptureTheFlag.ChechMoveFlag(client);

            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
        
            //Console.WriteLine(client.Map.ObjectMask[client.Player.X, client.Player.Y] + " " + client.Map.ObjectMask[client.Player.X, client.Player.Y]);
        }

        [DataAttribute(ActionType.DeleteCharacter)]
        public unsafe static void DeleteCharacter(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {

            if (client.Player.OnMyOwnServer == false)
                return;
            if (client.Player.SecurityPassword == data->dwParam)
            {
                if (client.Player.MyGuild != null)
                {
#if Arabic
                    client.SendSysMesage("Please remove your guild.");
#else
                    client.SendSysMesage("Please remove your guild.");
#endif
                    
                    return;
                }
                if (client.Player.MyClan != null)
                {
#if Arabic
                       client.SendSysMesage("Please remove your clan.");
#else
                    client.SendSysMesage("Please remove your clan.");
#endif
                 
                    return;
                }

                client.Player.Delete = true;
                client.Socket.Disconnect();
            }
            else
            {
#if Arabic
                 client.SendSysMesage("wrong password.");
#else
                client.SendSysMesage("wrong password.");
#endif
               
            }

        }
        [DataAttribute(ActionType.ChangeDirection)]
        public unsafe static void ChangeDirection(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.OnAutoAttack = false;
            client.Player.Angle = (Role.Flags.ConquerAngle)data->Fascing;
#if TEST
               Console.WriteLine("Direction -> " + data->Fascing + " " + client.Player.Angle.ToString()+ "");
#endif
            client.Player.View.SendView(msg.ActionCreate(data), true);
            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
        }
        [DataAttribute(ActionType.ChangeStance)]
        public unsafe static void ChangeStance(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.OnAutoAttack = false;
            client.Player.Action = (Role.Flags.ConquerAction)data->dwParam;

            if (client.Player.Action == Role.Flags.ConquerAction.Cool)
            {
                if (client.Equipment.FullSuper)
                    data->dwParam = (uint)(data->dwParam | (uint)(client.Player.Class * 0x10000 + 0x1000000));
                else if (client.Equipment.SuperArmor)
                    data->dwParam = (uint)(data->dwParam | (uint)(client.Player.Class * 0x10000));
            }
            client.Player.View.SendView(msg.ActionCreate(data), true);

            if (client.Player.ContainFlag(MsgUpdate.Flags.CastPray))
            {
                foreach (var user in client.Player.View.Roles(Role.MapObjectType.Player))
                {
                    if (Role.Core.GetDistance(client.Player.X, client.Player.Y, user.X, user.Y) <= 4)
                    {
                        data->Timestamp = (int)user.UID;
                        client.Player.View.SendView(msg.ActionCreate(data), true);
                    }
                }
            }
            if (client.Player.ActivePick)
                client.Player.RemovePick(msg);
        }
        [DataAttribute(ActionType.SetPkMode)]
        public unsafe static void SetPkMode(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
                client.SendSysMesage("Jianghu will turn of in 10 minutes.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            client.Player.PkMode = (Role.Flags.PKMode)data->dwParam;
            client.Send(msg.ActionCreate(data));
            if (client.Player.PkMode == Role.Flags.PKMode.PK)
                client.SendSysMesage("Free PK mode. You can attack monster and all players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Capture)
                client.SendSysMesage("Capture PK mode. You can only attack monsters, black-name and blue-name players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Peace)
                client.SendSysMesage("Peace mode. You can only attack monsters.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Team)
                client.SendSysMesage("Team PK mode. You can attack monster and all players except your teammates or guild.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Revange)
                client.SendSysMesage("Revenge PK mode. You can attack monster and all Your Enemy List Players.", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
            else if (client.Player.PkMode == Role.Flags.PKMode.Guild)
                client.SendSysMesage("Guild PK mode. You can attack monster and all Your Guild's Enemies", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#if Jiang
            if (client.Player.PkMode == Role.Flags.PKMode.Jiang)
            {
                if (client.Player.MyJiangHu != null)
                {
                    client.Player.MyJiangHu.ActiveJiangMode(client);
                    client.SendSysMesage("You have returned to the Jiang Hu !", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                }
            }
#endif
        }
        [DataAttribute(ActionType.ConfirmAssociates)]
        public unsafe static void ConfirmAssociates(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmSpells)]
        public unsafe static void ConfirmSpells(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.MySpells.SendAll(msg);
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmProficiencies)]
        public unsafe static void ConfirmProficiencies(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.MyProfs.SendAll(msg);
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.ConfirmGuild)]
        public unsafe static void ConfirmGuild(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            if (client.Player.MyGuild != null && client.Player.MyGuildMember != null)
            {
                client.Player.SendString(msg, Game.MsgServer.MsgStringPacket.StringID.GuildName, client.Player.MyGuild.Info.GuildID, true
                    , new string[1] { client.Player.MyGuild.GuildName + " " + client.Player.MyGuild.Info.LeaderName + " " + client.Player.MyGuild.Info.Level + " " + client.Player.MyGuild.Info.MembersCount + " 83" });

                client.Player.MyGuild.SendThat(client.Player);
                client.Player.SendString(msg, Game.MsgServer.MsgStringPacket.StringID.GuildName, client.Player.MyGuild.Info.GuildID, true
                   , new string[1] { client.Player.MyGuild.GuildName + " " + client.Player.MyGuild.Info.LeaderName + " " + client.Player.MyGuild.Info.Level + " " + client.Player.MyGuild.Info.MembersCount + " 83" });

                client.Player.MyGuild.SendGuildAlly(msg, true, client);
                client.Player.MyGuild.SendGuilEnnemy(msg, true, client);
        
              

                Game.MsgServer.MsgGuildMinDonations MinimDonation = new MsgGuildMinDonations(msg, 31);
                MinimDonation.AprendGuild(msg, client.Player.MyGuild);
                client.Send(MinimDonation.ToArray(msg));

                client.Player.GuildBattlePower = client.Player.MyGuild.ShareMemberPotency(client.Player.MyGuildMember.Rank);
            }
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.AllowAnimation)]
        public unsafe static void AllowAnimation(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Equipment.Show(msg);
            data->ObjId = client.Player.UID;
            client.Send(msg.ActionCreate(data));
        }
        [DataAttribute(ActionType.CompleteLogin)]
        public unsafe static void CompleteLogin(Client.GameClient client, ServerSockets.Packet msg, ActionQuery* data)
        {
            client.Player.CompleteLogin = true;
            if (client.OnInterServer)
            {
                client.Equipment.QueryEquipment(client.Equipment.Alternante);
                if (client.Player.InitTransfer == 1)
                {
                    client.Player.InitTransfer = 2;
                    Console.WriteLine("Complete Transfer for " + client.Player.Name);
                    client.Send(msg.InterServerCheckTransferCreate(3, client.Player.UID));
                    lock (Database.Server.NameUsed)
                    {
                        if (Database.Server.NameUsed.Contains(client.Player.Name.GetHashCode()))
                        {
                            if (!Database.Server.NameUsed.Contains(client.Player.UID.ToString().GetHashCode()))
                            {
                                Database.Server.NameUsed.Add(client.Player.UID.ToString().GetHashCode());
                            }
                            client.Player.Name = client.Player.UID.ToString();
                        }
                    }
                    
                }
            }
            else
            {
                client.Player.UpdateVip(msg);
#if Jiang
                if (client.Player.MyJiangHu != null)
                    client.Player.MyJiangHu.LoginClient(msg, client);
                else if (client.Player.Reborn == 2)
                {
                    client.Send(msg.JiangHuInfoCreate(MsgJiangHuInfo.JiangMode.IconBar, "0"));
                }
#endif
                client.MyWardrobe.SendToClient(msg);
                if (client.Player.GuildID == 0 && client.Player.Level > 20)
                {
#if Arabic
                 client.SendSysMesage("Why not join a guild. and find some companions in this turbulent world!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#else
                    client.SendSysMesage("Why not join a guild. and find some companions in this turbulent world!", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
#endif

                }
                client.CreatePrestigePoints();
                MsgUserAbilityScore.UserAbilityScore info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 1;
                info.Level = client.Player.Level;
                info.UID = client.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[19];
                for (int x = 0; x < 19; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = client.PrestigePoints[x];

                }
                client.Send(msg.UserAbilityScoreCreate(info));
            }
        }
    }
}