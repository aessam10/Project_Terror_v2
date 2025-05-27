using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.WebServer
{
    public static class Proces
    {
        public class Client
        {
            public ServerSockets.SecuritySocket SecuritySocket;
            public Client(ServerSockets.SecuritySocket _socket)
            {
                if (_socket.RemoteIp == Program.ServerConfig.AccServerIPAddres)
                {
                    Object = this;
                    _socket.Client = this;
                    SecuritySocket = _socket;
                }
                else
                    _socket.Disconnect();
            }

            public unsafe void Send(ServerSockets.Packet msg)
            {
                SecuritySocket.Send(msg);
            }
            public void Disconnect()
            {

            }
        }

        public static Client Object;
        public static ServerSockets.ServerSocket AccServer;
        public static ConnectionPoll PollConnections;

        public static bool UpdateDBTables = false;

        public static Extensions.Time32 AccServerStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.AccServerStamp);

        public static void Init()
        {
            if (Program.ServerConfig.IsInterServer == false)
            {
                PollConnections = new ConnectionPoll();

                AccServer = new ServerSockets.ServerSocket(
                     new Action<ServerSockets.SecuritySocket>(p => new Client(p))
                     , new Action<ServerSockets.SecuritySocket, ServerSockets.Packet>((p, data) =>
                     {
                         ProcesReceive(p, data);
                     })
                     , new Action<ServerSockets.SecuritySocket>(p => (p.Client as Client).Disconnect()));
                AccServer.Initilize(Program.ServerConfig.Port_SendSize, Program.ServerConfig.Port_ReceiveSize, 1, 3);
                AccServer.Open(Program.ServerConfig.AccServerIPAddres, Program.ServerConfig.WebPort, Program.ServerConfig.Port_BackLog);
            }
     
        }


        public static unsafe void Close()
        {
            if (Object != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    stream.InitWriter();
                    stream.Finalize(Packets.Closed);
                    Object.Send(stream);
                }
            }
        }
        public static unsafe void ProcesReceive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)
        {
            var Game = (obj.Client as Client);

            ushort PacketID = stream.ReadUInt16();

            switch (PacketID)
            {
                case Packets.Ping:
                    {
                        stream.InitWriter();
                        stream.Write(Database.Server.GamePoll.Values.Count);
                        stream.Finalize(Packets.Ping);
                        Object.Send(stream);
                        break;
                    }
                case Packets.LoginQueues:
                    {
                        uint UID = stream.ReadUInt32();
                        uint hash = stream.ReadUInt32();
                        PollConnections.Add(UID, hash);
                        break;
                    }
                case Packets.UpdateDb:
                    {
                        UpdateDatabase();
                        break;
                    }
                case Packets.CheckRecharge:
                    {
                        uint UID = stream.ReadUInt32();
                        uint Points = stream.ReadUInt32();



                        Project_Terror_v2.Client.GameClient _client;
                        if (Database.Server.GamePoll.TryGetValue(UID, out _client))
                        {
                            _client.Player.RechargePoints = Points;
                            _client.ActiveNpc = (uint)Project_Terror_v2.Game.MsgNpc.NpcID.RechargeShop;
                            Project_Terror_v2.Game.MsgNpc.NpcHandler.RechargeShop(_client, stream, 90, "", 0);
                        }
                        break;
                    }
                case Packets.CheckPrize://brb smoke
                    {
                        uint UserUID = stream.ReadUInt32();
                        uint PrizeUID = stream.ReadUInt32();
                        uint PrizeID = stream.ReadUInt32();
                        uint Claim = stream.ReadUInt32();

                        Project_Terror_v2.Client.GameClient user;
                        if (Database.Server.GamePoll.TryGetValue(UserUID, out user))
                        {
                            if (PrizeUID == 0)
                            {
                                user.CreateBoxDialog("Please come back later there is nothing for you to claim at the moment. ");
                            }
                            else
                            {
                                switch (PrizeUID)//faci la tine astea cel mai bine
                                {
#if Encore
                                                case 1: //99,999 CPs
                                        {
                                            user.Player.ConquerPoints += 99999;
                                            user.CreateBoxDialog("You have received 99,999 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 99,999 CPs bought from donate page.(Thanks for supporting our server).");

                                         
                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                          
                                            }

                                            break;
                                        }

                                    case 2: // 699,999 conquer points
                                        {
                                            user.Player.ConquerPoints += 699999;
                                            user.CreateBoxDialog("You have received 699,999 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 165,000 CPs bought from donate page.(Thanks for supporting our server).");

                                        
                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                              
                                            }

                                            break;
                                        }
        
                                    case 3://vip 6
                                        {
                                            /*
                                            user.Player.VipLevel = 6;
                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddDays(30);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddDays(30);
                                            user.Player.SendUpdate(stream, user.Player.VipLevel, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                                            user.Player.UpdateVip(stream);

                                            user.CreateBoxDialog("You have received VIP6(30 Days). Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  VIP 6 bought from donate page.(Thanks for supporting our server).");
                                          
                                            */

                                            user.Inventory.Add(stream, 780000, 1);
                                            user.CreateBoxDialog("You have received VIP6(Token). Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  VIP 6 bought from donate page.(Thanks for supporting our server).");
                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                              
                                            }
                                            break;
                                        }



                                    case 4:// 
                                        {

                                            user.Inventory.Add(stream, 192935, 1, 0, 1);
                                            user.CreateBoxDialog("You have received EvilPumpkin . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  EvilPumpkin  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 5:// 
                                        {
                                            user.Inventory.Add(stream, 188755, 1, 0, 1);
                                            user.CreateBoxDialog("You have received TenderFlameGarment . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  TenderFlameGarment  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                 
                                    case 6:// 
                                        {
                                            user.Inventory.Add(stream, 193275);

                                            user.CreateBoxDialog("You have received RobeOfDarkness(hades) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  RobeOfDarkness(hades)  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 7:// 
                                        {
                                            user.Inventory.Add(stream, 193605);

                                            user.CreateBoxDialog("You have received FieryRedUniform . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  FieryRedUniform  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 8:// 
                                        {
                                            user.Inventory.Add(stream, 193625);

                                            user.CreateBoxDialog("You have received ButterflyRose . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  ButterflyRose  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 9:// 
                                        {
                                            user.Inventory.Add(stream, 193795);

                                            user.CreateBoxDialog("You have received ValiantHero . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  Valianthero  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 10:// 
                                        {
                                            user.Inventory.Add(stream, 188915);

                                            user.CreateBoxDialog("You have received GoldCloth . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  GoldCloth  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 11:// 
                                        {
                                            user.Inventory.Add(stream, 193525);

                                            user.CreateBoxDialog("You have received FrozenFantasy . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  FrozenFantasy  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 12:// 
                                        {
                                            user.Inventory.Add(stream, 193445);

                                            user.CreateBoxDialog("You have received FrozenFantasy . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  FrozenFantasy  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 13:// 
                                        {
                                            user.Inventory.Add(stream, 193225);

                                            user.CreateBoxDialog("You have received PrideOfSuccess . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  PrideOfSuccess  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 14:// 
                                        {
                                            user.Inventory.Add(stream, 193565);

                                            user.CreateBoxDialog("You have received LavaCatRobe . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  LavaCatRobe  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 15://
                                        {
                                            user.Inventory.Add(stream, 192785);

                                            user.CreateBoxDialog("You have received FlameDance . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  FlameDance  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



                                    case 16:// 
                                        {
                                            user.Inventory.Add(stream, 193785);

                                            user.CreateBoxDialog("You have received LionofFury(Legend) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her LionofFury(Legend) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 17:// 
                                        {
                                            user.Inventory.Add(stream, 193805);

                                            user.CreateBoxDialog("You have received BlueResurgence(Legend) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her BlueResurgence(Legend) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 18:// 
                                        {
                                            user.Inventory.Add(stream, 193815);

                                            user.CreateBoxDialog("You have received GermanyJersey(Legend) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her GermanyJersey(Legend) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 19:// 
                                        {
                                            user.Inventory.Add(stream, 193825);

                                            user.CreateBoxDialog("You have received SpainJersey(Legend) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her SpainJersey(Legend) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 20:// 
                                        {
                                            user.Inventory.Add(stream, 193555);

                                            user.CreateBoxDialog("You have received LavaCatRobe(Charm) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her LavaCatRobe(Charm) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 21:// 
                                        {
                                            user.Inventory.Add(stream, 192615);

                                            user.CreateBoxDialog("You have received ImperialRobe . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her ImperialRobe bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 22:// 
                                        {
                                            user.Inventory.Add(stream, 193295);

                                            user.CreateBoxDialog("You have received SongofDespair . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her SongofDespair bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 23:// 
                                        {
                                            user.Inventory.Add(stream, 189095);

                                            user.CreateBoxDialog("You have received CloudRobe . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her CloudRobe bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 24:// 
                                        {
                                            user.Inventory.Add(stream, 188185);

                                            user.CreateBoxDialog("You have received SoulofSword . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her SoulofSword bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 25:// 
                                        {
                                            user.Inventory.Add(stream, 193325);

                                            user.CreateBoxDialog("You have received AspirationJacket . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her AspirationJacket bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 26:// 
                                        {
                                            user.Inventory.Add(stream, 193205);

                                            user.CreateBoxDialog("You have received PrideofTrumph . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her PrideofTrumph bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



                                    case 27:// 
                                        {
                                            user.Inventory.Add(stream, 193715);

                                            user.CreateBoxDialog("You have received FlushofHearts . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her FlushofHearts bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                        //=============> START OF MOUNTS

                                    case 28:// 
                                        {
                                            user.Inventory.Add(stream, 200531);

                                            user.CreateBoxDialog("You have received FieryDragon . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  FieryDragon  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 29:// 
                                        {
                                            user.Inventory.Add(stream, 200517);

                                            user.CreateBoxDialog("You have received KingOfScorpions . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  KingOfScorpions  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



                                    case 30:// 
                                        {
                                            user.Inventory.Add(stream, 200560);

                                            user.CreateBoxDialog("You have received LunarMonkey . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  LunarMonkey  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



                                    case 31:// 
                                        {
                                            user.Inventory.Add(stream, 200537);

                                            user.CreateBoxDialog("You have received HolyLotus . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  HolyLotus  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 32:// 
                                        {
                                            user.Inventory.Add(stream, 200449);

                                            user.CreateBoxDialog("You have received Somersaultcloud . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  Somersaultcloud  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 33:// 
                                        {
                                            user.Inventory.Add(stream, 200564);

                                            user.CreateBoxDialog("You have received SuperVictorAlpaca . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her SuperVictorAlpaca bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }


                                    case 34:// 
                                        {
                                            user.Inventory.Add(stream, 200574);

                                            user.CreateBoxDialog("You have received SnowLotus . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her SnowLotus bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 35:// 
                                        {
                                            user.Inventory.Add(stream, 200543);

                                            user.CreateBoxDialog("You have received GeneralCat(Charm) . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her GeneralCat(Charm) bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }

                                    case 36:// 
                                        {
                                            user.Inventory.Add(stream, 200494);

                                            user.CreateBoxDialog("You have received MoneyHorse . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her MoneyHorse bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }
                                    case 37:// 
                                        {
                                            user.Inventory.Add(stream, 200575);

                                            user.CreateBoxDialog("You have received AstralPhoenix . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  AstralPhoenix  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



                                    case 38:// 
                                        {
                                            user.Inventory.Add(stream, 200573);

                                            user.CreateBoxDialog("You have received RadiantLotus . Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));



                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  RadiantLotus  bought from donate page.(Thanks for supporting our server).");

                                            if (user.Player.ClaimStateGift == 0)
                                            {
                                                user.Player.ClaimStateGift |= Role.Player.ClaimGiftFlag.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.ClaimStateGift, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);

                                            }
                                            break;
                                        }



#else
                                    case 1035:
                                        {
                                            user.Inventory.Add(stream, 824020);
                                            string ItemName = "CraneBoots";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1034:
                                        {
                                            user.Inventory.Add(stream, 824019);
                                            string ItemName = "DragonBoots";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1033:
                                        {
                                            user.Inventory.Add(stream, 824018);
                                            string ItemName = "FoxBoots";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1032:
                                        {
                                            user.Inventory.Add(stream, 823062);
                                            string ItemName = "TigerHeavyRing";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1031:
                                        {
                                            user.Inventory.Add(stream, 823061);
                                            string ItemName = "LionHeavyRing";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1030:
                                        {
                                            user.Inventory.Add(stream, 823060);
                                            string ItemName = "RainbowBracelet";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1029:
                                        {
                                            user.Inventory.Add(stream, 823059);
                                            string ItemName = "DragonRing";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1028:
                                        {
                                            user.Inventory.Add(stream, 823058);
                                            string ItemName = "CraneRing";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1027:
                                        {
                                            user.Inventory.Add(stream, 822072);
                                            string ItemName = "EclipseArmor";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1026:
                                        {
                                            user.Inventory.Add(stream, 822071);
                                            string ItemName = "NetherArmor";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1025:
                                        {
                                            user.Inventory.Add(stream, 821034);
                                            string ItemName = "FervorBag";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1024:
                                        {
                                            user.Inventory.Add(stream, 821033);
                                            string ItemName = "HeavenNecklace";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1023:
                                        {
                                            user.Inventory.Add(stream, 820076);
                                            string ItemName = "IceHeadgear";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1022:
                                        {
                                            user.Inventory.Add(stream, 820075);
                                            string ItemName = "StarHeadgear";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1021:
                                        {
                                            user.Inventory.Add(stream, 820074);
                                            string ItemName = "MoonHeadgear";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1020:
                                        {
                                            user.Inventory.Add(stream, 820073);
                                            string ItemName = "MoonHeadgear";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1019:
                                        {
                                            user.Inventory.Add(stream, 800917);
                                            string ItemName = "GhostKnife";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1018:
                                        {
                                            user.Inventory.Add(stream, 800811);
                                            string ItemName = "RepentRapier";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1017:
                                        {
                                            user.Inventory.Add(stream, 800810);
                                            string ItemName = "DeathPistol";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1016:
                                        {
                                            user.Inventory.Add(stream, 800725);
                                            string ItemName = "BuddaBeads";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1015:
                                        {
                                            user.Inventory.Add(stream, 800618);
                                            string ItemName = "SunBow";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1014:
                                        {
                                            user.Inventory.Add(stream, 800522);
                                            string ItemName = "TimeBacksword";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1013:
                                        {
                                            user.Inventory.Add(stream, 800422);
                                            string ItemName = "SpiritShield";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1012:
                                        {
                                            user.Inventory.Add(stream, 800255);
                                            string ItemName = "DemonScythe";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1011:
                                        {
                                            user.Inventory.Add(stream, 800215);
                                            string ItemName = "SkyHalberd";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1010:
                                        {
                                            user.Inventory.Add(stream, 800142);
                                            string ItemName = "ShadowKatana";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1009:
                                        {
                                            user.Inventory.Add(stream, 800111);
                                            string ItemName = "SkyHammer";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1008:
                                        {
                                            user.Inventory.Add(stream, 800020);
                                            string ItemName = "MonsterSaber";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1007:
                                        {
                                            user.Inventory.Add(stream, 801004);
                                            string ItemName = "WarCraze";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1040:
                                        {
                                            user.Inventory.Add(stream, 3100055);
                                            string ItemName = "GarmentPack";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1041:
                                        {
                                            user.Inventory.Add(stream, 3100054);
                                            string ItemName = "MountArmorPack";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1042:
                                        {
                                            user.Inventory.Add(stream, 3100053);
                                            string ItemName = "AccesoryPack";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1043:
                                        {
                                            user.Inventory.Add(stream, 3100052);
                                            string ItemName = "BlackFridayPack";
                                            user.CreateBoxDialog("You have received " + ItemName + ". Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her " + ItemName + " bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " " + ItemName + " bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            }
                                            break;
                                        }
                                    case 1000: // 100k conquer points
                                        {
                                            user.Player.ConquerPoints += 100000;
                                            user.CreateBoxDialog("You have received 100,000 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 100,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 100,000CPs bought from donate page.(Thanks for supporting our server).");

                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 100,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                         //       user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }

                                            break;
                                        }
                                    case 1001://220k conquer points
                                        {
                                            user.Player.ConquerPoints += 220000;
                                            user.CreateBoxDialog("You have received 220,000 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 220,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 220,000CPs bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 220,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                            //    user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;

                                        }
                                    case 1002://550 conquer points
                                        {
                                            user.Player.ConquerPoints += 550000;
                                            user.CreateBoxDialog("You have received 550,000 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 550,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 550,000CPs bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 550,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                                //    user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }

                                    case 2001://2kk conquer points
                                        {
                                            user.Player.ConquerPoints += 2000000;
                                            user.CreateBoxDialog("You have received 2,000,000 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 2,000,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 2,000,000CPs bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 2,000,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) != Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                                //    user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }
                                    case 2002://5kk conquer points
                                        {
                                            user.Player.ConquerPoints += 5000000;
                                            user.CreateBoxDialog("You have received 5,000,000 CPs. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 5,000,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her 5,000,000CPs bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " 5,000,000CPs bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) != Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                                //    user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }
                                    case 100://vip 6 2 hour
                                        {
                                            user.Player.VipLevel = 6;
                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddHours(1);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddHours(1);
                                            user.Player.SendUpdate(stream, user.Player.VipLevel, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                                            user.Player.UpdateVip(stream);

                                            user.CreateBoxDialog("You have received 2 RechargePoints and 1hour Vip6. Thank you for your support.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));

                                          
                                            break;
                                        }
                                    case 1004:
                                        {
                                            user.UpdateLevel(stream, 140, true, false);
                                           

                                            user.CreateBoxDialog("You have received Level 140. Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));


                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Level 140 bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  Level 140 bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Level 140 bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                                //   user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }
                                    case 1036://vip 6
                                        {
                                            user.Player.VipLevel = 6;
                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddDays(3);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddDays(3);
                                            user.Player.SendUpdate(stream, user.Player.VipLevel, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                                            user.Player.UpdateVip(stream);

                                            user.CreateBoxDialog("You have received VIP6(3 Days). Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));


                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Vip6 bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  VIP 6(3 Days) bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Vip6(3 Days) bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                                //   user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }
                                    case 1003://vip 6
                                        {
                                           /* user.Player.VipLevel = 6;
                                            if (DateTime.Now > user.Player.ExpireVip)
                                                user.Player.ExpireVip = DateTime.Now.AddDays(30);
                                            else
                                                user.Player.ExpireVip = user.Player.ExpireVip.AddDays(30);
                                            user.Player.SendUpdate(stream, user.Player.VipLevel, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.VIPLevel);
                                            user.Player.UpdateVip(stream);*/

                                            user.Inventory.Add(stream, 780000, 1);
                                            user.CreateBoxDialog("You have received VIP6(Token). Thank you for your donation.");
                                            Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));


                                            string amessaj = "";
                                            if (Role.Core.IsBoy(user.Player.Body))
                                                amessaj = "his";
                                            else if (Role.Core.IsGirl(user.Player.Body))
                                                amessaj = "her";
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Vip6(Token) bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.Center);
                                            MyConsole.WriteLine(user.Player.Name + " ! claimed her  VIP 6 bought from donate page.(Thanks for supporting our server).");
                                            Program.SendGlobalPackets.Enqueue(messaj.GetArray(stream));
                                            Project_Terror_v2.Game.MsgServer.MsgMessage messaj2 = new Project_Terror_v2.Game.MsgServer.MsgMessage(user.Player.Name + " ! claimed " + amessaj + " Vip6(Token) bought from donate page.(Thanks for supporting our server).", Project_Terror_v2.Game.MsgServer.MsgMessage.MsgColor.red, Project_Terror_v2.Game.MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                                            Program.SendGlobalPackets.Enqueue(messaj2.GetArray(stream));
                                            if ((user.Player.MainFlag & Role.Player.MainFlagType.CanClaim) !=  Role.Player.MainFlagType.CanClaim)
                                            {
                                                user.Player.MainFlag |= Role.Player.MainFlagType.CanClaim;
                                                user.Player.SendUpdate(stream, (uint)user.Player.MainFlag, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.MainFlag, false);
                                             //   user.Player.SendUpdate(stream, Project_Terror_v2.Game.MsgServer.MsgUpdate.CreditGifts.ShowSpecialItems, Project_Terror_v2.Game.MsgServer.MsgUpdate.DataType.CreditGifts, false);
                                            }
                                            break;
                                        }
                                  

#endif
                                   /* case 4009://gold prize
                                        {
                                            if (user.Inventory.HaveSpace(1))
                                            {
                                                user.Inventory.Add(stream, 2100075, 1, 0, 1);
                                                user.CreateBoxDialog("You`ve receive one Gold Prize(-1) . Thank for you donation.");
                                                Object.Send(stream.ClaimPrize(UserUID, PrizeUID, PrizeID, 1));
                                            }
                                            else
                                                user.CreateBoxDialog("Please make 1 space in your inventory.");
                                            break;
                                        }*/
                                }
                            }
                        }
                        break;
                    }
            }


            ServerSockets.PacketRecycle.Reuse(stream);
        }

        public static ServerSockets.Packet ClaimPrize(this ServerSockets.Packet stream, uint UserUID, uint PrizeUID, uint PrizeID, uint Claim)
        {
            stream.InitWriter();
            stream.Write(UserUID);
            stream.Write(PrizeUID);
            stream.Write(PrizeID);
            stream.Write(Claim);
            stream.Finalize(Packets.CheckPrize);
            return stream;
        }
        public static ServerSockets.Packet CheckRecharge(this ServerSockets.Packet stream, uint UserUID)
        {
            stream.InitWriter();
            stream.Write(UserUID);
            stream.Finalize(Packets.CheckRecharge);
            if (Object != null)
            {

                Object.Send(stream);
               
            }
            return stream;
        }
        public unsafe static void work(Extensions.Time32 clock)
        {
            if (clock > AccServerStamp)
            {
                if (!UpdateDBTables)
                {
                    UpdateDatabase();
                }
                AccServerStamp.Value = clock.Value + KernelThread.AccServerStamp;
            }
        }
        public unsafe static void CheckPrize(ServerSockets.Packet stream, uint UID)
        {
            if (Object != null)
            {
                Object.Send(stream.ClaimPrize(UID, 0, 0, 0));
            }
        }
        public unsafe static void UpdateDatabase()
        {
            if (Object != null)
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    for (Database.RanksTable.TopType i = Database.RanksTable.TopType.Trojan; i < Database.RanksTable.TopType.Count; i++)
                    {
                        var arrayPlayers = Database.RanksTable.ArrayTops[i];
                        stream.InitWriter();

                        stream.Write((uint)i);
                        stream.Write(arrayPlayers.Count);
                        if (i == Database.RanksTable.TopType.ConquerPoints)
                        {
                            for (int x = 0; x < arrayPlayers.Count; x++)
                            {
                                var element = arrayPlayers[x];
                                stream.Write(element.Class);
                                stream.Write(element.Level);
                                stream.Write((ulong)element.ConquerPoints);
                                stream.Write(element.Name, 16);
                                //Console.WriteLine(i.ToString() + " = " + arrayPlayers[x].Class + " " + arrayPlayers[x].Name + " " + arrayPlayers[x].NobilityDonation);
                            }
                        }
                        else
                        {
                            for (int x = 0; x < arrayPlayers.Count; x++)
                            {
                                var element = arrayPlayers[x];
                                stream.Write(element.Class);
                                stream.Write(element.Level);
                                stream.Write(element.NobilityDonation);
                                stream.Write(element.Name, 16);
                                //Console.WriteLine(i.ToString() + " = " + arrayPlayers[x].Class + " " + arrayPlayers[x].Name + " " + arrayPlayers[x].NobilityDonation);
                            }
                        }
                        stream.Finalize(Packets.UpdatePlayersRanks);
                        Object.Send(stream);
                    }

                    stream.InitWriter();
                    stream.Write(0);
                    stream.Write(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.Name, 16);

                    Role.Instance.Guild winnerguild;
                    if (Role.Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID, out winnerguild))
                        stream.Write(winnerguild.Info.LeaderName, 16);
                    else
                        stream.Write("None", 16);

                    stream.Finalize(Packets.UpdateGwWinner);
                    Object.Send(stream);


                    stream.InitWriter();
                    stream.Write(1);
                    stream.Write(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Winner.Name, 16);

                    if (Role.Instance.Guild.GuildPoll.TryGetValue(Game.MsgTournaments.MsgSchedules.SuperGuildWar.Winner.GuildID, out winnerguild))
                        stream.Write(winnerguild.Info.LeaderName, 16);
                    else
                        stream.Write("None", 16);

                    stream.Finalize(Packets.UpdateGwWinner);
                    Object.Send(stream);


                    stream.InitWriter();
                    var array = Role.Instance.Guild.GuildPoll.Values.Where(p => p.CTF_Exploits != 0).OrderByDescending(p => p.CTF_Exploits).ToArray();
                    stream.Write(Math.Min(array.Length, 9));
                    for (int x = 0; x < Math.Min(array.Length, 9); x++)
                    {
                        var element = array[x];
                        stream.Write(element.GuildName,16);
                        stream.Write(element.Info.LeaderName, 16);
                        stream.Write(element.CTF_Exploits);
                    }
                    stream.Finalize(Packets.UpdateCTF);
                    Object.Send(stream);

                    UpdateDBTables = true;
                }
            }

        }
    }
}
