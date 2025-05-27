using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgServer
{
    public struct MsgLoginClient
    {
        public ushort Length;
        public ushort PacketID;
        public uint AccountHash;
        public uint Key;

        [PacketAttribute(Game.GamePackets.LoginGame)]
        public unsafe static void LoginGame(Client.GameClient client, ServerSockets.Packet packet)
        {
            client.OnLogin = new MsgLoginClient()
            {
                AccountHash =packet.ReadUInt32(),
                Key = packet.ReadUInt32()
            };
            client.ClientFlag |= Client.ServerFlag.OnLoggion;
            Database.ServerDatabase.LoginQueue.TryEnqueue(client);
        }
        public unsafe static void LoginHandler(Client.GameClient client, MsgLoginClient packet)
        {
            client.ClientFlag &= ~Client.ServerFlag.OnLoggion;
           
            if (client.Socket != null)
            {
                if (client.Socket.RemoteIp == "NONE")
                {
                    MyConsole.WriteLine("Breack login client.");
                    return;
                }
            }
            try
            {
                string BanMessaje;
                if (Database.SystemBanned.IsBanned(client.Socket.RemoteIp,out BanMessaje))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

#if Arabic
                          string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#else
                        string Messaj = "You IP Address is Banned for: " + BanMessaje + " ";
#endif
                      
                        client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                    }
                    return;
                }
                if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) == Client.ServerFlag.CreateCharacterSucces)
                {
                    if (Database.ServerDatabase.AllowCreate(client.ConnectionUID))
                    {
                   
                        client.ClientFlag &= ~Client.ServerFlag.CreateCharacterSucces;
                        if (client.Player.MyChi == null)
                        {
                            client.Player.MyChi = new Role.Instance.Chi(client.Player.UID);

                        }
                        if (client.Player.SubClass == null)
                            client.Player.SubClass = new Role.Instance.SubClass();
                        if (client.Player.Flowers == null)
                        {
                            client.Player.Flowers = new Role.Instance.Flowers(client.Player.UID, client.Player.Name);
                            client.Player.Flowers.FreeFlowers = 1;
                        }
                        if (client.Player.Nobility == null)
                            client.Player.Nobility = new Role.Instance.Nobility(client);
                        if (client.Player.Associate == null)
                        {
                            client.Player.Associate = new Role.Instance.Associate.MyAsociats(client.Player.UID);
                            client.Player.Associate.MyClient = client;
                            client.Player.Associate.Online = true;
                        }
                        if (client.Player.InnerPower == null)
                           client.Player.InnerPower = new Role.Instance.InnerPower(client.Player.Name, client.Player.UID);

                        Database.Server.GamePoll.TryAdd(client.Player.UID, client);
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
                            client.Send(new MsgServer.MsgMessage("ANSWER_OK", "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));


                            Database.ServerDatabase.CreateCharacte(client);
                            Database.ServerDatabase.SaveClient(client);

                            client.ClientFlag |= Client.ServerFlag.AcceptLogin;

                            MyConsole.WriteLine("Client " + client.Player.Name + " was login on [" + client.Socket.RemoteIp + "]");

                            client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                            MsgLoginHandler.LoadMap(client, stream);
                        }
                        return;
                    }
                }
                if ((client.ClientFlag & Client.ServerFlag.AcceptLogin) != Client.ServerFlag.AcceptLogin)
                {

                   var login = client.OnLogin;
                    //MsgLoginClient* login = (MsgLoginClient*)&packet;

                    client.ConnectionUID = login.Key;


                    if (Database.SystemBannedAccount.IsBanned(client.ConnectionUID, out BanMessaje))
                    {

                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
#if Arabic
                             string aMessaj = "Your account is Banned for: " + BanMessaje + " ";
#else
                            string aMessaj = "Your account is Banned for: " + BanMessaje + " ";
#endif
                           
                            client.Send(new MsgServer.MsgMessage(aMessaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                        }
                        return;
                    }

                    string Messaj = "NEW_ROLE";

                    if (WebServer.Proces.PollConnections.CheckJoin(client.ConnectionUID, login.AccountHash))
                    {

                        if (Database.ServerDatabase.AllowCreate(login.Key) == false)
                        {

                            Client.GameClient InGame = null;
                            if (Database.Server.GamePoll.TryGetValue((uint)login.Key, out InGame))
                            {
                                if (InGame.Player != null)
                                {
                                    MyConsole.WriteLine("Account try join but is in use. " + InGame.Player.Name);

                                    if (InGame.Player.UID == 0)
                                    {
                                        Database.Server.GamePoll.TryRemove((uint)login.Key, out InGame);
                                        if (InGame != null && InGame.Player != null)
                                        {
                                            if (InGame.Map != null)
                                                InGame.Map.Denquer(InGame);
                                        }
                                    }
                                }
                                InGame.Socket.Disconnect();
#if Arabic
                                     Messaj = "Sorry, you Account is online, Try Again";
#else
                                Messaj = "Sorry, you Account is online, Try Again";
#endif
                           
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                    client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                                }
                                if (InGame.TRyDisconnect-- == 0)
                                {
                                    if (InGame.Player != null && InGame.FullLoading)
                                    {
                                        InGame.ClientFlag |= Client.ServerFlag.Disconnect;
                                        //if ((InGame.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                        Database.ServerDatabase.SaveClient(InGame);
                                    }
                                    Database.Server.GamePoll.TryRemove((uint)login.Key, out InGame);
                                    if (InGame != null && InGame.Player != null)
                                    {
                                        if (InGame.Map != null)
                                            InGame.Map.Denquer(InGame);
                                    }
                                }
                            }
                            else
                            {
                                Database.Server.GamePoll.TryAdd((uint)login.Key, client);
                                Messaj = "ANSWER_OK";
                                using (var rec = new ServerSockets.RecycledPacket())
                                {
                                    var stream = rec.GetStream();
                                  

                                    if ((client.ClientFlag & Client.ServerFlag.CreateCharacterSucces) != Client.ServerFlag.CreateCharacterSucces)
                                    {
                                        //  lock (client.Player)
                                        //      client.Player = new Role.Player(client);
                                        Database.ServerDatabase.LoadCharacter(client, (uint)login.Key);
                                    }
                                    client.ClientFlag |= Client.ServerFlag.AcceptLogin;
                                    MyConsole.WriteLine("Client " + client.Player.Name + " was login on [" + client.Socket.RemoteIp + "]");
                                    client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                                    client.Send(stream.LoginHandlerCreate(1, client.Player.Map));
                                    MsgLoginHandler.LoadMap(client, stream);
                                }
                            }
                        }
                        else//new client
                        {
                            client.ClientFlag |= Client.ServerFlag.CreateCharacter;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));
                            }
                        }
                    }
                    else
                    {
#if Encore
                        //
#else
                        Console.WriteLine("Nice try , The Protection is Active!");
#endif
                        using (var rec = new ServerSockets.RecycledPacket())
                        {
                            var stream = rec.GetStream();
#if Arabic
                               Messaj = "Nice try , The Protection is Active! ";
#else
                            Messaj = "Nice try , The Protection is Active! ";
#endif
                         
                            client.Send(new MsgServer.MsgMessage(Messaj, "ALLUSERS", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Dialog).GetArray(stream));

                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
    }
}
