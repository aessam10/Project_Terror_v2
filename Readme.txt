sa adaug mesajele la nobility
si sa adaug mesajele la observing Equipment
Basta is observing your equipment!,


sa adaug sa nu dea trade la bound items, lock....
sa adauga la shop sa nu le vanda
sa adaug jar la shop sa nu`l vanda
sa adaug jar la trade sa nu`l dea


sa adaug trnasformarea la item in epic la trojan !!!!
sa adaug npc la Guild War in jail.
sa adaug watch team arena!!!
sa fac la lev mic attacu !!!
sa fac la lev mic sa dea items cand creezi accounts!
sa fac la lev mic sa dea la monk sa nu mearga 2 blades ....
sa fac la lev mic sa ii apara paru in cap
.
sa maresc procentu la level.



Elite Pk.
{
-scriu packetele;
-sa fac verificarea doar pe mapa aia .
-sa fac sa nu intre la arena.
-sa fac fac verificarea la arena Qualiffer.





}

Capture the flag
{

public byte[] generateX2Packet(uint owner, ushort x = 0, ushort y = 0)
        {
            byte[] data = new byte[48];
            Writer.WriteInt32(data.Length - 8, 0, data);
            Writer.WriteUInt16(2224, 2, data);
            Writer.WriteInt32(X2Location, 4, data);
            Writer.WriteUInt32(owner, 8, data);
            Writer.WriteInt32(1, 28, data);
            Writer.WriteUInt16(x, 32, data);
            Writer.WriteUInt16(y, 34, data);
            return data;
        }
}


ultima schimbare in source ! ce am de verificat

public unsafe void CheckScren()


de schimbat aparatele 
MsgAdvertiseOpenGui

safe bool Update(Game.MsgServer.MsgGameItem ItemDat, AddMode mode, ServerSockets.Packet strea, bool Removefull = false)

si gameitem 