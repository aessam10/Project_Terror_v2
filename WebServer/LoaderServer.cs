using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Project_Terror_v2.Game;
using Project_Terror_v2.Game.MsgServer;

namespace Project_Terror_v2.WebServer
{
    public class LoaderServer
    {

        public class Client
        {
            public enum Flags : ulong
            {
                None = 0,
                Aim = 1UL << 1,
                Speed = 1UL << 2,
                Clicker = 1UL << 3,
                SuspectProgram = 1UL << 4

            }
            public string name = "";
            public uint UID = 0;
            public Flags MyFlags = Flags.None;
            public ServerSockets.SecuritySocket SecuritySocket;
            public object SyncRoot;
            public Client(ServerSockets.SecuritySocket _socket)
            {
                SyncRoot = new object();
                _socket.Client = this;
                SecuritySocket = _socket;
            }

            public unsafe void Send(ServerSockets.Packet msg)
            {
                SecuritySocket.Send(msg);
            }

            public void Disconnect()
            {
                if (Clients.ContainsKey(UID))
                {
                    Client user;
                    Clients.TryRemove(UID, out user);
                    SecuritySocket.Disconnect();
                }
            }
        }
        public static ServerSockets.ServerSocket Server;
        public static ConnectionPoll PollConnections;
        public static bool CanCheck = false;
        public static DateTime TimerStamp = DateTime.Now.AddMinutes(60);
        public static Extensions.Counter Counter = new Extensions.Counter(10);
        public static System.Collections.Concurrent.ConcurrentDictionary<uint, Client> Clients = new System.Collections.Concurrent.ConcurrentDictionary<uint, Client>();
        public static string CheckIP = "";
        public static char[] Chars = new char[] { 'A', 'B', 'C', 'D', 'R', 'T', 'Y', 'U', 'I', 'U', 'K', 'I', 'p', 'T', 'I', 'N', 'M', 'T', 'R', 'E', 'w', 'Q' };
        public static string LogginKey = "";
        public static void CreateLogginKey()
        {//C238xs65pjy7HU9Q";
            LogginKey = "C238xs65pjy7HU9Q";
            //  return;
            for (int i = 0; i < 16; i++)
            {
                LogginKey += Chars[Program.GetRandom.Next(0, Chars.Length)];
            }

        }
        public static void Init()
        {
            if (Program.ServerConfig.IsInterServer == false)
            {
                CreateLogginKey();
                PollConnections = new ConnectionPoll();

                Server = new ServerSockets.ServerSocket(
                     new Action<ServerSockets.SecuritySocket>(p => new Client(p))
                     , new Action<ServerSockets.SecuritySocket, ServerSockets.Packet>((p, data) =>
                     {
                         ProcesReceive(p, data);
                     })
                     , new Action<ServerSockets.SecuritySocket>(p => (p.Client as Client).Disconnect()));
                Server.Initilize(Program.ServerConfig.Port_SendSize, Program.ServerConfig.Port_ReceiveSize, 1, 3);
                Server.Open(Program.ServerConfig.LoaderIP, Program.ServerConfig.LoaderPort, Program.ServerConfig.Port_BackLog);

            }
        }
        public unsafe static void CheckPrograms(string name, string addres)
        {
            foreach (var obj in Clients.Values)
            {

                if (obj.SecuritySocket.RemoteIp == addres)
                {

                    obj.name = name;

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        stream.InitWriter();
                        stream.Write(0);
                        stream.Finalize(2000);
                        obj.Send(stream);

                    }
                    break;
                }

            }

        }
        public static int procount = 0;
        public static unsafe void ProcesReceive(ServerSockets.SecuritySocket obj, ServerSockets.Packet data)
        {
            var Game = (obj.Client as Client);
            uint packetid = data.ReadUInt16();

            switch (packetid)
            {
                case 1099:
                    {
                        try
                        {
                            ulong Flag = data.ReadUInt64();
                            Game.MyFlags = (Client.Flags)Flag;
                            byte[] datastr = data.ReadBytes(100);
                            string program = System.Text.ASCIIEncoding.ASCII.GetString(datastr).Replace("\0", "");
                            program = program.Split('.')[0];
                            program += ".exe";
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Socket.RemoteIp == Game.SecuritySocket.RemoteIp)
                                {

                                    if (Flag == 1ul << 4)
                                    {
                                        uint BanHours = 0;
                                        if (user.BanCount == 0)
                                            BanHours = 24 * 1;
                                        else if (user.BanCount == 1)
                                            BanHours = 24 * 7;
                                        else if (user.BanCount == 2)
                                            BanHours = 24 * 14;
                                        else
                                            BanHours = 24 * 364;
                                        user.BanCount += 1;
                                        Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, BanHours, program);
                                        string Messaje = "" + user.Player.Name + " got banned for " + BanHours / 24 + " days, because was found using programs that are illegal in game (" + program + ").";
                                        Game.MsgServer.MsgMessage msg = new MsgMessage(Messaje, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                                        Program.SendGlobalPackets.Enqueue(msg.GetArray(data));
                                    }
                                    user.Socket.Disconnect();
                                    Console.WriteLine(user.Player.Name + " <---- " + Flag.ToString() + " " + program);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                        break;
                    }
                case 1300:
                    {

                        byte leng = data.ReadUInt8();
                        byte[] datastr = data.ReadBytes(leng);
                        string program = System.Text.ASCIIEncoding.ASCII.GetString(datastr);
                        string directory = Environment.CurrentDirectory + @"\\Programs\\" + DateTime.Now.DayOfYear.ToString() + " of " + DateTime.Now.Year + ".txt";

                        using (var SW = File.AppendText(directory))
                        {
                            SW.WriteLine("IP[" + obj.RemoteIp + "] open " + program);

                            SW.Close();
                        }

                        break;
                    }
                case 1400:
                    {


                        data.InitWriter();
                        for (int x = 0; x < LogginKey.Length; x++)
                            data.Write(Convert.ToByte(LogginKey[x]));
                        data.Finalize(1401);
                        obj.Send(data);

                        break;
                    }
                case 1999:
                    {
                        data.InitWriter();
                        data.Finalize(1999);
                        obj.Send(data);

                        if (Game.UID == 0)
                        {
                            Game.UID = Counter.Next;
                            Clients.TryAdd(Game.UID, Game);
                        }
                        break;
                    }
                case 3000:
                    {

                        //   byte leng = data.ReadUInt8();
                        byte[] datastr = data.ReadBytes(100);
                        string program = System.Text.ASCIIEncoding.ASCII.GetString(datastr).Replace("\0", "");
                        program = program.Split('.')[0];
                        program += ".exe";
                      string c_szText = program;
                        if (program.Contains("Le Bot") || program.Contains("MouseAndKeyboardRecorder.exe") 
                            || program.Contains("Mouse") && program.Contains("Recorder") || program.Contains("Cheat Engine")
                            || program.Contains("cheatengine") || program.Contains("AutoClick") || program.Contains("Auto Click")
                            || program.Contains("Auto_Click") || program.Contains("Auto") && program.Contains("Click")
                            || program.Contains("Aimbot") || program.Contains("Aim") && program.Contains("bot")
                            || program.Contains("COBot"))
                        {
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Socket.RemoteIp == Game.SecuritySocket.RemoteIp)
                                {

                                    uint BanHours = 0;
                                    if (user.BanCount == 0)
                                        BanHours = 24 * 1;
                                    else if (user.BanCount == 1)
                                        BanHours = 24 * 7;
                                    else if (user.BanCount == 2)
                                        BanHours = 24 * 14;
                                    else
                                        BanHours = 24 * 364;
                                    user.BanCount += 1;
                                    Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, BanHours, program);
                                    string Messaje = "" + user.Player.Name + " got banned for " + BanHours / 23 + " days, because was found using programs that are illegal in game (" + program + ").";
                                    Game.MsgServer.MsgMessage msg = new MsgMessage(Messaje, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                                    Program.SendGlobalPackets.Enqueue(msg.GetArray(data));

                                    user.Socket.Disconnect();
                                    Console.WriteLine(user.Player.Name + " <---- was banned " + program);
                                }
                            }
                            break;
                        }
                        else if (strcmp("AutoClicker.exe", c_szText) == 0
                    || strcmp("AMC.exe", c_szText) == 0
                    || strcmp("COELSE.exe", c_szText) == 0
                    || strcmp("co2latro.exe", c_szText) == 0
                    || strcmp("auto hunter.exe", c_szText) == 0
                    || strcmp("Auto miner.exe", c_szText) == 0
                    || strcmp("Aimbot.exe", c_szText) == 0
                    || strcmp("Auto potter.exe", c_szText) == 0
                    || strcmp("CoGenius.exe", c_szText) == 0
                    || strcmp("Melee.exe", c_szText) == 0
                    || strcmp("GCO~Next.exe", c_szText) == 0
                    || strcmp("Co~nex.exe", c_szText) == 0
                    || strcmp("hooker.exe", c_szText) == 0
                    || strcmp("winject.exe", c_szText) == 0
                    || strcmp("COoperative.exe", c_szText) == 0
                    || strcmp("AmcEngine.exe", c_szText) == 0
                    || strcmp("GSAutoClicker.exe", c_szText) == 0
                    || strcmp("AutoClicker vs2.exe", c_szText) == 0
                    || strcmp("cheatengine-x86_64.exe", c_szText) == 0
                    || strcmp("cheatengine-i386.exe", c_szText) == 0
                    || strcmp("Cheat Engine.exe", c_szText) == 0
                    || strcmp("Conquer Clickyr.exe", c_szText) == 0
                    || strcmp("Clicky.exe", c_szText) == 0
                    || strcmp("Klick0r.exe", c_szText) == 0
                    || strcmp("TinyTask.exe", c_szText) == 0
                    || strcmp("ReMouse.exe", c_szText) == 0
                    || strcmp("multidades1.0.exe", c_szText) == 0
                    || strcmp("AutoClickExtreme6.exe", c_szText) == 0
                    || strcmp("AutoClickLil.exe", c_szText) == 0
                    || strcmp("Autosofter Auto Mouse Clicker1.7.exe", c_szText) == 0
                    || strcmp("ConquerClickerZ.exe", c_szText) == 0
                    || strcmp("Ghost Mouse Auto Clicker.exe", c_szText) == 0
                    || strcmp("QoProxy.exe", c_szText) == 0
                    || strcmp("AutoClicker3.4.1.exe", c_szText) == 0
                    || strcmp("AimBot-Carniato.exe", c_szText) == 0
                    || strcmp("AutoClicker2.4.exe", c_szText) == 0
                    || strcmp("Autosofted Mouse Clicker 1.5.exe", c_szText) == 0
                    || strcmp("Rotation Pilot.exe", c_szText) == 0
                    || strcmp("Clicker.exe", c_szText) == 0
                    || strcmp("AutoClicker v2.4.exe", c_szText) == 0
                    || strcmp("Clicker.exe", c_szText) == 0
                    || strcmp("Conquer Clicky 2.0.exe", c_szText) == 0
                    || strcmp("5792 Tools.exe", c_szText) == 0
                    || strcmp("Alalawi9.exe", c_szText) == 0
                    || strcmp("Win32 Cabinet Self-Extractor.exe", c_szText) == 0
                    || strcmp("Conquer Effect Remover.exe", c_szText) == 0
                    || strcmp("AutoClickTG 1.1.exe", c_szText) == 0
                    || strcmp("GameHackerPM COBot.exe", c_szText) == 0
                    || strcmp("Tam.exe", c_szText) == 0
                    || strcmp("My Clicker 3.exe", c_szText) == 0
                    || strcmp("Auto-Pot.exe", c_szText) == 0
                    || strcmp("AutoClicker v2.exe", c_szText) == 0
                    || strcmp("simpleclicker.exe", c_szText) == 0
                    || strcmp("CoAuto.exe", c_szText) == 0
                    || strcmp("PixelBot.exe", c_szText) == 0
                    || strcmp("ASLCtJ v3.5.exe", c_szText) == 0
                    || strcmp("ArcherBuddy1.0.exe", c_szText) == 0
                    || strcmp("Lazy for the win!.exe", c_szText) == 0
                    || strcmp("Clickster.exe", c_szText) == 0
                    || strcmp("CoClick.exe", c_szText) == 0
                    || strcmp("Conquer Clicky.exe", c_szText) == 0
                    || strcmp("Hacer Conquer online Arbic 2014 v7.exe", c_szText) == 0
                    || strcmp("Speed Gear Setup.exe", c_szText) == 0
                    || strcmp("ItemType Modder.exe", c_szText) == 0
                    || strcmp("CoGenius.exe", c_szText) == 0
                    || strcmp("COELSE.exe", c_szText) == 0
                    || strcmp("ConquerAI.exe", c_szText) == 0
                    || strcmp("UnionTeamsInjector.exe", c_szText) == 0
                    || strcmp("CoAimbot.exe", c_szText) == 0
                    || strcmp("COHelper.exe", c_szText) == 0
                    || strcmp("ColourBot.exe", c_szText) == 0
                    || strcmp("Conquer Online.exe", c_szText) == 0
                    || strcmp("line.exe", c_szText) == 0
                    || strcmp("x to donate2.exe", c_szText) == 0
                    || strcmp("CO-Hack.exe", c_szText) == 0
                    || strcmp("LineOptv2.exe", c_szText) == 0
                    || strcmp("CheatEngine.exe", c_szText) == 0
                    || strcmp("Jtbit.exe", c_szText) == 0
                    || strcmp("MacroRecorder.exe", c_szText) == 0
                    || strcmp("AutoClickerV2.exe", c_szText) == 0
                    || strcmp("Vico.exe", c_szText) == 0
                    || strcmp("Mouse Recorder Pro.exe", c_szText) == 0
                    || strcmp("Mouse Recorder Pro2.exe", c_szText) == 0
                    || strcmp("netcutAddProgram.exe", c_szText) == 0
                            || c_szText.Contains("cheatengine")
                            )
                        {
                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Socket.RemoteIp == Game.SecuritySocket.RemoteIp)
                                {

                                    uint BanHours = 0;
                                    if (user.BanCount == 0)
                                        BanHours = 24 * 1;
                                    else if (user.BanCount == 1)
                                        BanHours = 24 * 7;
                                    else if (user.BanCount == 2)
                                        BanHours = 24 * 14;
                                    else
                                        BanHours = 24 * 364;
                                    user.BanCount += 1;
                                    Database.SystemBannedAccount.AddBan(user.Player.UID, user.Player.Name, BanHours, program);
                                    string Messaje = "" + user.Player.Name + " got banned for " + BanHours / 23 + " days, because was found using programs that are illegal in game (" + program + ").";
                                    Game.MsgServer.MsgMessage msg = new MsgMessage(Messaje, MsgMessage.MsgColor.red, MsgMessage.ChatMode.System);
                                    Program.SendGlobalPackets.Enqueue(msg.GetArray(data));

                                    user.Socket.Disconnect();
                                    Console.WriteLine(user.Player.Name + " <---- was banned " + program);
                                }
                            }
                            break;
                        }
                        if (CheckIP == obj.RemoteIp)
                        {
                            string directory = Environment.CurrentDirectory + @"\\GMCheckPrograms\\" + Game.name + "" + DateTime.Now.DayOfYear.ToString() + " of " + DateTime.Now.Year + ".txt";

                            using (var SW = File.AppendText(directory))
                            {
                                SW.WriteLine(program);

                                SW.Close();
                            }
                        }
                        break;
                    }
            }
            ServerSockets.PacketRecycle.Reuse(data);
        }
        public static int strcmp(string name, string name2)
        {
            if (name == name2)
                return 0;
            return 1;
        }
    }
}
