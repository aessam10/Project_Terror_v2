﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer.AttackHandler
{
    public class Attack
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (user.Equipment.RightWeaponEffect == Role.Flags.ItemEffect.Poison)
            {
                if (Calculate.Base.Rate(20) || Attack.SpellID == (ushort)Role.Flags.SpellID.Poison)
                {
                    Poison.Execute(user,Attack, stream, DBSpells);
                    return;
                }
            }
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user,DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.TripleBlasts:
                        {

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                    {
                                        MsgSpell clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                        {
                                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                            {
                                                Database.MagicType.Magic spell;
                                                if (Spells.TryGetValue(clientspell.Level, out spell))
                                                {
                                                    if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                    {
                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);

                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                    {
                                        MsgSpell clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                        {
                                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                            {
                                                Database.MagicType.Magic spell;
                                                if (Spells.TryGetValue(clientspell.Level, out spell))
                                                {
                                                    if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                    {
                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.ContainFlag(MsgUpdate.Flags.ShadowofChaser))
                                    {
                                        MsgSpell clientspell;
                                        if (user.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out clientspell))
                                        {
                                            Dictionary<ushort, Database.MagicType.Magic> Spells;
                                            if (Database.Server.Magic.TryGetValue((ushort)Role.Flags.SpellID.ShadowofChaser, out Spells))
                                            {
                                                Database.MagicType.Magic spell;
                                                if (Spells.TryGetValue(clientspell.Level, out spell))
                                                {
                                                    if (AttackHandler.Calculate.Base.Rate(spell.Rate))
                                                    {
                                                        Attack.SpellID = (ushort)Role.Flags.SpellID.ShadowofChaser;
                                                        AttackHandler.KineticSpark.AttackSpell(user, Attack, stream, Spells);
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.FlyingMoon:
                        {
                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.TwofoldBlades:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                           
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

             
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SuperTwofoldBlade:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        if (distance < 3)
                                        {
                                            AnimationObj.Damage = (uint)(AnimationObj.Damage * 1.35);
                                        }
                                        Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                   
                                        ReceiveAttack.Player.Execute(AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    short distance = Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y);
                                    if (distance <= DBSpell.Range)
                                    {
                                        MsgSpellAnimation.SpellObj AnimationObj;
                                        Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                        AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                        if (distance < 3)
                                        {
                                            AnimationObj.Damage = (uint)(AnimationObj.Damage * 1.35);
                                        }
                                        Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);

                                        MsgSpell.Targets.Enqueue(AnimationObj);
                                    }
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.EagleEye:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , user.Player.UID, Attack.X,Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                    Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);

                                    if (attacked.BlackSpot)
                                    {
                                        if (Calculate.Base.Rate(80))
                                        {
                                            MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                    , 0, user.Player.X, user.Player.Y, 11130
                                    , 4, 0);
                                            RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                            RemoveCloudDown.SetStream(stream);
                                            RemoveCloudDown.JustMe(user);
                                        }
                                    }
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);

                                    if (attacked.BlackSpot)
                                    {
                                        if (Calculate.Base.Rate(80))
                                        {
                                            MsgSpellAnimation RemoveCloudDown = new MsgSpellAnimation(user.Player.UID
                                    , 0, user.Player.X, user.Player.Y, 11130
                                    , 4, 0);
                                            RemoveCloudDown.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { UID = user.Player.UID, Damage = 11030, Hit = 1 });
                                            RemoveCloudDown.SetStream(stream);
                                            RemoveCloudDown.JustMe(user);
                                        }
                                    }

                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);


                                    Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                                    MsgSpell.SetStream(stream);
                                    MsgSpell.Send(user);

                                  
                                   
                                }
                                Updates.IncreaseExperience.Up(stream,user, Experience);
                            }
                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.RapidFire:
                    case (ushort)Role.Flags.SpellID.MortalWound:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                       
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                 
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Range.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                           if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                           {
                               var attacked = target as Role.SobNpc;
                               if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                               {
                                   MsgSpellAnimation.SpellObj AnimationObj;
                                   Calculate.Range.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                   AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                   Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                   MsgSpell.Targets.Enqueue(AnimationObj);
                               }
                           }
                           Updates.IncreaseExperience.Up(stream,user, Experience);
                           Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.DragonFury:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            
                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                   

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    if (user.Player.BattlePower > attacked.BattlePower)
                                    {
                                        attacked.AddSpellFlag(MsgUpdate.Flags.DragonFury, (int)DBSpell.Duration, true);
                                        attacked.SendUpdate(stream,Game.MsgServer.MsgUpdate.Flags.DragonFury, DBSpell.Duration
           , (uint)33, ClientSpell.Level, Game.MsgServer.MsgUpdate.DataType.DragonFury, true);
                                    }

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                   

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                         
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.SpeedKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.ViolentKick;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);
                                 
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ViolentKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                ,0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.StormKick;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                           
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.Tornado:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                              , 0, Attack.X, Attack.Y, ClientSpell.ID
                              , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                  //  AnimationObj.Damage = 1;//for test
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);

                                    Sering.Proces(user, attacked, stream);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                               
                                    Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream, user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);


/*                            if (!user.Equipment.FreeEquip((Role.Flags.ConquerItem)4) && user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SearingTouch))
                            {

                                MsgGameItem BackSword;
                                if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.RightWeapon, out BackSword))
                                {
                                    if (Database.ItemType.IsTaoistEpicWeapon(BackSword.ITEM_ID))
                                    {
                                        if (user.Player.TaoistPower < 10)
                                        {
                                            user.Player.TaoistPower += 1;
                                            user.Player.UpdateTaoPower(stream);
                                        }

                                        byte change = 10;

                                        MsgGameItem hossus;
                                        if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.LeftWeapon, out hossus))
                                        {
                                            if (Database.ItemType.IsHossu(hossus.ITEM_ID))
                                            {
                                                var dbItem = Database.Server.ItemsBase[hossus.ITEM_ID];
                                                change += (byte)(dbItem.Level / 4);
                                            }
                                        }

                                        if (AttackHandler.Calculate.Base.Rate(change))
                                        {
                                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                                            {
                                                Role.Player attacked = target as Role.Player;

                                                if(attacked.ContainFlag(MsgUpdate.Flags.lianhuaran04))
                                                {
                                                    Attack.SpellID = (ushort)Role.Flags.SpellID.SearingTouch;
                                                    var Spell = Database.Server.Magic[Attack.SpellID];

                                                    ClientSpell = user.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.SearingTouch];
                                                    MsgSpell = new MsgSpellAnimation(attacked.UID
                               , 0, Attack.X, Attack.Y, ClientSpell.ID 
                               , ClientSpell.Level, ClientSpell.UseSpellSoul,1);


                                                    foreach (Role.IMapObj atarget in attacked.View.Roles(Role.MapObjectType.Monster))
                                                    {
                                                        
                                                        MsgMonster.MonsterRole aattacked = atarget as MsgMonster.MonsterRole;
                                                        if (CheckAttack.CanAttackMonster.Verified(user, aattacked, DBSpell))
                                                        {
                                                            if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, aattacked.X, aattacked.Y) < DBSpell.Range)
                                                            {
                                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                                Calculate.Magic.OnMonster(user.Player, aattacked, DBSpell, out AnimationObj);
                                                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                                                Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, aattacked);
                                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                    foreach (Role.IMapObj atarger in attacked.View.Roles(Role.MapObjectType.Player))
                                                    {
                                                        var aattacked = atarger as Role.Player;
                                                        if (CheckAttack.CanAttackPlayer.Verified(user, aattacked, DBSpell))
                                                        {
                                                            if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, aattacked.X, aattacked.Y) < DBSpell.Range)
                                                            {
                                                                MsgSpellAnimation.SpellObj AnimationObj;
                                                                Calculate.Magic.OnPlayer(user.Player, aattacked, DBSpell, out AnimationObj);
                                                                AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                                                ReceiveAttack.Player.Execute(AnimationObj, user, aattacked);
                                                                MsgSpell.Targets.Enqueue(AnimationObj);
                                                            }
                                                        }
                                                    }
                                                   
                                                    Updates.IncreaseExperience.Up(stream, user, Experience);
                                                    Updates.UpdateSpell.CheckUpdate(stream, user, Attack, Experience, DBSpells);
                                                    MsgSpell.Create(stream); 
                                                    MsgSpell.Send(user);


                                                    attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran04);
                                                }
                                                else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran03))
                                                {
                                                    attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran03);
                                                    attacked.AddFlag(MsgUpdate.Flags.lianhuaran04,6,true);
                                                }
                                                else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran02))
                                                {
                                                    attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran02);
                                                    attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran03, 6, true);
                                                }
                                               else if (attacked.ContainFlag(MsgUpdate.Flags.lianhuaran01))
                                                {
                                                    attacked.RemoveFlag(MsgUpdate.Flags.lianhuaran01);
                                                    attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran02, 6, true);
                                                }
                                                else
                                                    attacked.AddSpellFlag(MsgUpdate.Flags.lianhuaran01, 6, true);
                                            }
                                        }
                                    }
                                }
                            }


                            */

                            break;
                        }
                    case (ushort)Role.Flags.SpellID.StormKick:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.NextSpell = (ushort)Role.Flags.SpellID.StormKick;

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }

                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    user.Shift(attacked.X, attacked.Y,stream);

                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);
                         
                            break;
                        }
                    default:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               ,0, Attack.X, Attack.Y, ClientSpell.ID
                               , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            uint Experience = 0;
                            Role.IMapObj target;
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Monster))
                            {
                                MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                                if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience+=     ReceiveAttack.Monster.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                  
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.Player))
                            {
                                var attacked = target as Role.Player;
                                if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            if (user.Player.View.TryGetValue(Attack.OpponentUID, out target, Role.MapObjectType.SobNpc))
                            {
                                var attacked = target as Role.SobNpc;
                                if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                                {
                                    MsgSpellAnimation.SpellObj AnimationObj;
                                    Calculate.Magic.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                                    AnimationObj.Damage = Calculate.Base.CalculateSoul(AnimationObj.Damage, ClientSpell.UseSpellSoul);
                                    Experience += ReceiveAttack.Npc.Execute(stream,AnimationObj, user, attacked);
                                    MsgSpell.Targets.Enqueue(AnimationObj);
                                }
                            }
                            Updates.IncreaseExperience.Up(stream,user, Experience);
                            Updates.UpdateSpell.CheckUpdate(stream,user,Attack, Experience, DBSpells);
                            MsgSpell.SetStream(stream); MsgSpell.Send(user);

                            break;
                        }
                }
            }
        }

    }
}
