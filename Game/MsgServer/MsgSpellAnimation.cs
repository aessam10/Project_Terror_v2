﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Project_Terror_v2.Game.MsgServer
{
    public class MsgSpellAnimation : IDisposable
    {
        public class SpellObj
        {
            public static SpellObj ShallowCopy(SpellObj obj)
            {
                return (SpellObj)obj.MemberwiseClone();
            }
            public uint UID;
            public uint Damage;
            public uint Hit;
            public MsgAttackPacket.AttackEffect Effect;
            public uint MoveX;
            public uint MoveY;

            public uint x1;
            public uint y2;
            public uint xx1;
            public uint yy2;
            public uint xxx1;
            public uint yyy2;

            public SpellObj()
            {
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage
                ,MsgAttackPacket.AttackEffect _effect)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
            }
            public SpellObj(uint _target, uint _damage
               , MsgAttackPacket.AttackEffect _effect,ushort x, ushort y)
            {
                UID = _target;
                Damage = _damage;
                Effect = _effect;
                Hit = 1;
                MoveY = y;
                MoveX = x;
                x1 = xx1 = xx1 = x;
                y2 = yy2 = yyy2 = y;

            }
        }
      //  public List<ServerSockets.Packet> Arrays;

        public Queue<SpellObj> Targets;
        public uint UID;
        public uint OpponentUID;
        public ushort X;
        public ushort Y;
        public ushort SpellID;
        public byte LevelSoul;
        public ushort SpellLevel;
        public ushort NextSpell;
        public uint bomb = 0;

        public MsgSpellAnimation()
        {
            Targets = new Queue<SpellObj>();
        }
        public MsgSpellAnimation(uint _uid, uint oponnent
            , ushort _x, ushort _y, ushort _spell, ushort _spelllevel, byte _levelsoul, uint boomb =0)
        {
            Targets = new Queue<SpellObj>();
          
            UID = _uid;
            OpponentUID = oponnent;
            X = _x;
            Y = _y;
            SpellID = _spell;
            SpellLevel = _spelllevel;
            LevelSoul = _levelsoul;
            bomb = boomb;
        }
        private unsafe ServerSockets.Packet CreateAnimation(Queue<SpellObj> Spells, ServerSockets.Packet stream)
        {
            stream.InitWriter();
            stream.Write(UID);//4
            if (OpponentUID != 0)
                stream.Write(OpponentUID);//8
            else
            {
                stream.Write(X);
                stream.Write(Y);
            }
            stream.Write(SpellID);//12
            stream.Write(SpellLevel);//14
            stream.Write(NextSpell);//16

            stream.Write(LevelSoul);//18

            stream.Write((byte)Targets.Count);//19

            stream.Write((uint)bomb);

            // SpellObj Obj;
            foreach (var Obj in Spells.ToArray())
             {
                 //if (Obj.Damage >= 1)
                 {
                     stream.Write(Obj.UID);
                     stream.Write(Obj.Damage);
                     stream.Write(Obj.Hit);
                     stream.Write((uint)Obj.Effect);
                     stream.Write(0);//unknow
                     stream.Write(Obj.MoveX);
                     stream.Write(Obj.MoveY);
                     stream.Write(0);//unknow
                 }
             }
         
             stream.Finalize(Game.GamePackets.SpellUse);
             return stream;
        }
        public ServerSockets.Packet _Stream;
        public unsafe void SetStream(ServerSockets.Packet stream)
        {
            _Stream = stream;
       
        }
        private bool TryDequeue(Queue<SpellObj> Data, out SpellObj obj)
        {
            if (Data.Count != 0)
            {
                obj = Data.Dequeue();
                return true;
            }
            obj = null;
            return false;
        }
        public void JustMe(Client.GameClient user)
        {
            if (Targets.Count < 30)
            {
                user.Send(CreateAnimation(Targets, _Stream));
            }
            else
            {
                Dictionary<uint, Queue<SpellObj>> BigArray = new Dictionary<uint, Queue<SpellObj>>();
                //BigArray.Add(0, new Queue<SpellObj>());
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Send(CreateAnimation(small_array, _Stream));

            }
        }
        public void Send(Client.GameClient user, bool self = true)
        {
            if (Targets.Count < 30)
            {
                user.Player.View.SendView(CreateAnimation(Targets, _Stream), self);
            }
            else
            {
                Dictionary<uint, Queue<SpellObj>> BigArray = new Dictionary<uint, Queue<SpellObj>>();
                //BigArray.Add(0, new Queue<SpellObj>());
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Player.View.SendView(CreateAnimation(small_array, _Stream), self);

            }
        }
       
        public void SendRole(Client.GameClient user)
        {
            if (Targets.Count < 30)
            {
                user.Player.View.Role(false, CreateAnimation(Targets, _Stream));
            }
            else
            {
                Dictionary<uint, Queue<SpellObj>> BigArray = new Dictionary<uint, Queue<SpellObj>>();
              //  BigArray.Add(0, new Queue<SpellObj>());
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)
                    {
                        count++;
                        BigArray.Add(count, new Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    user.Player.View.Role(false, CreateAnimation(small_array, _Stream));

            }
        }
        public void Send(MsgMonster.MonsterRole monster)
        {
            if (Targets.Count < 30)
            {
                monster.Send(CreateAnimation(Targets, _Stream));
            }
            else
            {
                Dictionary<uint, Queue<SpellObj>> BigArray = new Dictionary<uint, Queue<SpellObj>>();
             //   BigArray.Add(0, new Queue<SpellObj>());
                var TargetsArray = Targets.ToArray();
                uint count = 0;
                for (int x = 0; x < TargetsArray.Length; x++)
                {
                    if (x % 30 == 0)// x % 30 = 0
                    {
                        count++;
                        BigArray.Add(count, new Queue<SpellObj>());
                    }
                    BigArray[count].Enqueue(TargetsArray[x]);
                }
                foreach (var small_array in BigArray.Values)
                    monster.Send(CreateAnimation(small_array, _Stream));

            }

        }
        public void Dispose()
        {
            Targets = null;
        }
    }
}
