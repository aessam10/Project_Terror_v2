﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Project_Terror_v2.ServerSockets
{
    public unsafe class SecuritySocket
    {
        public ReceiveBuffer ReceiveBuff;
        public bool IsGameServer
        {
            get
            {
                return Crypto != null;
            }
        }

        public bool SetDHKey = false;
   
        public object SendRoot;
        private Action<SecuritySocket> OnDisconnect;
        private Action<SecuritySocket, Packet> OnReceiveHandler;
        public Socket Connection;
        public object Client;

        private Queue<byte[]> OnSend;
        private IDisposable[] TimerSubscriptions = null;
        public bool Alive = false;
        public Cryptography.TQCast5 Crypto;
        public bool OnInterServer = false;
        public string RemoteIp
        {
            get
            {
                try
                {
                    if (Connection == null)
                        return "NONE";
                    return (Connection.RemoteEndPoint as IPEndPoint).Address.ToString();
                }
                catch
                {
                    return "NONE";
                }
            }
        }
        public Extensions.Time32 LastReceive;
        public Client.GameClient Game;
        public ServerSocket Server;

        public bool ConnectFull = false;

        public SecuritySocket(ServerSocket serversocket, Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            Server = serversocket;
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
           
        
        }
        public SecuritySocket( Action<SecuritySocket> _OnDisconnect, Action<SecuritySocket, Packet> _OnReceiveHandler)
        {
            OnReceiveHandler = _OnReceiveHandler;
            OnDisconnect = _OnDisconnect;
        }
        public bool Connect(string IPAddres, ushort port, out Socket _socket)
        {
             _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IAsyncResult asyncResult = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IPAddres), port), null, null);
            uint count = 0;
            while (!asyncResult.IsCompleted && count < 10)
            {
                count++;
                System.Threading.Thread.Sleep(100);
            }
            if (asyncResult.IsCompleted)
            {
               
                _socket.Blocking = false;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                // Set option that allows socket to receive out-of-band information in the data stream.
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.OutOfBandInline, true);
                _socket.ReceiveBufferSize = _socket.SendBufferSize = ServerSockets.ReceiveBuffer.RECV_BUFFER_SIZE;
            
            }
            return asyncResult.IsCompleted;
        }
        public void Create(Socket _socket)
        {
            try
            {
                ReceiveBuff = new ReceiveBuffer(Program.ServerConfig.Port_ReceiveSize);
                Connection = _socket;
                SetDHKey = false;
                Alive = true;
                LastReceive = Extensions.Time32.Now;
                SendRoot = new object();
                OnSend = new Queue<byte[]>();
                Client = Crypto = null;
                TimerSubscriptions = new IDisposable[]
      {
              ThreadPoll.Subscribe<SecuritySocket>(ThreadPoll.ConnectionReceive, this, ThreadPoll.ReceivePool),
                ThreadPoll.Subscribe<SecuritySocket>(ThreadPoll.ConnectionSend, this, ThreadPoll.SendPool),
               ThreadPoll.Subscribe<SecuritySocket>(ThreadPoll.ConnectionReview, this, ThreadPoll.GenericThreadPool)
      };
             
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
        }
        public void SetCrypto(Cryptography.TQCast5 Crypt)
        {
            if (!Program.ServerConfig.IsInterServer)
            {
                Crypto = Crypt;
            }
        }
        public static void TryReview(SecuritySocket sock)
        {
            sock.CheckUp();

        }
        public void CheckUp()
        {
            if (Crypto != null)
            {
                if (Extensions.Time32.Now > LastReceive.AddSeconds(60 * 5))
                    this.Disconnect();
            }
        }


        public int tstgg = 0;

        public unsafe void Send(Packet msg)
        {
            try
            {
                if (Alive)
                {
                    lock (SendRoot)
                    {
                        
                        byte[] _buffer = new byte[msg.Size];
                        if (Crypto != null)
                            Crypto.Encrypt(msg.Memory, 0, _buffer, 0, msg.Size);
                        else
                            fixed (byte* ptr = _buffer)
                                msg.memcpy(ptr, msg.Memory, msg.Size);

                        OnSend.Enqueue(_buffer);
                    }
                }
            }
            catch (Exception e)
            {
                MyConsole.WriteLine(e.ToString());
            }
        }
        public ReceiveBuffer DHKeyBuffer = new ReceiveBuffer(1024, true);
        public ReceiveBuffer EncryptedDHKeyBuffer = new ReceiveBuffer(1024, true);
        public bool IsCompleteDHKey(out int type)
        {

            type = 0;
            try
            {
                if (DHKeyBuffer.Length() < 8)
                    return false;

                byte[] buffer = new byte[Packet.SealSize];
                for (int x = 0; x < buffer.Length; x++)
                    buffer[x] = DHKeyBuffer.buffer[x + (DHKeyBuffer.Length() - Packet.SealSize)];

                string Text = System.Text.ASCIIEncoding.ASCII.GetString(DHKeyBuffer.buffer);

                bool accept = Text.Contains("TQClient");
                if (!Text.EndsWith("TQClient"))
                    type = 1;
                return accept;
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
                Disconnect();
                return false;
            }
        }
        public unsafe bool ReceiveDHKey()
        {
            try
            {
            
            
            if (Alive && WindowsAPI.ws2_32.CanRead(this))
                {
                    int rec_type = 0;
                    if (!SetDHKey && Alive)
                    {
                        SocketError Socket_Error = SocketError.IsConnected;
                        int length = DHKeyBuffer.MaxLength() - DHKeyBuffer.Length();
                        if (length <= 0)
                        {
                            Disconnect();
                            return false;
                        }
                        int ret = Connection.Receive(DHKeyBuffer.buffer, DHKeyBuffer.Length(), length, SocketFlags.None, out Socket_Error);
                         if (ret > 0)
                         {
                             System.Buffer.BlockCopy(DHKeyBuffer.buffer, DHKeyBuffer.Length(), EncryptedDHKeyBuffer.buffer, EncryptedDHKeyBuffer.Length(), ret);
                             EncryptedDHKeyBuffer.AddLength(ret);
                             if (Crypto != null)
                             {
                                 fixed (byte* ptr = DHKeyBuffer.buffer)
                                     Crypto.Decrypt(DHKeyBuffer.buffer, DHKeyBuffer.Length(), ptr, DHKeyBuffer.Length(), ret);
                             }
                             DHKeyBuffer.AddLength(ret);
                             if (IsCompleteDHKey(out rec_type))
                             {
                                
                                 using (var rec = new RecycledPacket())
                                 {
                                     var stream = rec.GetStream();
                                     stream.Seek(0);
                                     fixed (byte* ptr = DHKeyBuffer.buffer)
                                         stream.memcpy(stream.Memory, ptr, DHKeyBuffer.Length());

                                     stream.Size = DHKeyBuffer.Length();

                                     if (OnReceiveHandler != null)
                                         OnReceiveHandler.Invoke(this, stream);
                                 }
                             }
                         }
                         else if (ret <= 0 || Socket_Error == SocketError.ConnectionAborted)
                         {
                             Disconnect();
                             return false;
                         }
                     
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect();
            }
            return false;
        }
        public bool ReceiveBuffer()
        {
            if (Alive)
            {
           
                {
                    try
                    {
                        if (!SetDHKey && Crypto != null)
                        {
                            ReceiveDHKey();
                        }
                        else if (WindowsAPI.ws2_32.CanRead(this))
                        {
                            try
                            {
                                if (!Alive)
                                    return false;
                               int available = Connection.Available;
                                if (available == 0)
                                {
                                   Disconnect();
                                   return false;
                                }
                                if (!Receive(available))
                                {
                                    return false;
                                }

                            }
                            catch (Exception e)
                            {
                                MyConsole.SaveException(e);
                                Disconnect();
                            }
                            return true;
                        }


                    }
                    catch (Exception e) { Disconnect(); Console.WriteLine(e.ToString()); }
                }
            }
            return false;
        }

        public unsafe bool HandlerBuffer()
        {
            int counts = 30;
            while (true && counts > 0)
            {
                counts--;
                if (!Alive)
                    return false;
                try
                {
                 
                    if (!ConnectFull)
                        return false;
                    if (ReceiveBuff.Length() == 0)
                        return false;
                    int Length = (int)(ReceiveBuff.ReadHead() + (IsGameServer ? 8 : 0));
                    if (Program.ServerConfig.IsInterServer || OnInterServer)
                        Length += 8;

                    if (Length < 2)
                        return false;
                    if (Length > ServerSockets.ReceiveBuffer.HeadSize)
                    {
                        Disconnect();
                        return false;
                    }
                    if (Length > ReceiveBuff.Length())
                        return false;
                    
                    LastReceive = Extensions.Time32.Now;
                    Packet Stream = PacketRecycle.Take();

                    Stream.Seek(0);

                    fixed (byte* ptr = ReceiveBuff.buffer)
                    {
                        Stream.memcpy(Stream.stream, ptr, Length);
                        if (Length < ReceiveBuff.Length())
                        {
                            fixed (void* next_buffer = &ReceiveBuff.buffer[Length])
                            {
                                Stream.memcpy(ptr, next_buffer, ReceiveBuff.Length() - Length);
                            }
                        }
                        Stream.Size = Length; 
                        
                        ReceiveBuff.DelLength(Length);
                    }

                    Stream.SeekForward(2);

                    if (OnReceiveHandler != null)
                        OnReceiveHandler.Invoke(this, Stream);

                }
                catch (Exception e) { Console.WriteLine(e.ToString()); }
            }
            return false;
        }
     
        public bool Receive(int available)
        {
            if (Alive)
            {
                SocketError Socket_Error = SocketError.IsConnected;
                try
                {
                    int length = ReceiveBuff.MaxLength() - ReceiveBuff.Length();
                  
                    int receive_size = 0;
                    if (available > length)
                        receive_size = length;
                    else
                        receive_size = available;
                    if (receive_size == 0)
                    {
                        return false;
                    }
                    int ret = Connection.Receive(ReceiveBuff.buffer, ReceiveBuff.Length(), receive_size, SocketFlags.None, out Socket_Error);
               
                    if (ret > 0)
                    {

                        if (Crypto != null)
                        {
                            fixed (byte* ptr = ReceiveBuff.buffer)
                                Crypto.Decrypt(ReceiveBuff.buffer, ReceiveBuff.Length(), ptr, ReceiveBuff.Length(), ret);
                        }
                        ReceiveBuff.AddLength(ret);
                      
                        return true;
                    }
                    else if (ret == 0)//<= 0 || Socket_Error == SocketError.ConnectionAborted)
                    {
                        Disconnect();
                        return false;
                    }
                    else//ret < 0
                    {
                        if (Socket_Error != SocketError.WouldBlock)
                            Disconnect();
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Console.WriteLine(Socket_Error.ToString());

                }
            }
            return false;
        }


        public bool CanDequeue(out byte[] data)
        {
            data = null;

          
            if (OnSend.Count > 0)
            {
                lock (SendRoot)
                    data = OnSend.Dequeue();
            }
            return data != null;
        }

       public int nPutBytes = 0;
       public byte[] packet = null;
       public static bool TrySend(SecuritySocket _socket)
       {
           if (_socket.Alive)
           {

               if (_socket.OnSend.Count > 2500)
               {
                   Console.WriteLine("the sync is " + _socket.OnSend.Count + "");
                   _socket.Disconnect();
                   return false;
               }
               SocketError sError = SocketError.Success;
               int ret = 0;
               if (_socket.packet != null)
               {
                //   if (WindowsAPI.ws2_32.CanWrite(_socket))
                   {
                       try
                       {
                           int nLen = _socket.packet.Length;
                           ret = _socket.Connection.Send(_socket.packet, _socket.nPutBytes, nLen - _socket.nPutBytes, SocketFlags.None, out sError);
                           if (ret > 0)
                           {
                               _socket.nPutBytes += ret;//Connection are NonBlocking = true
                               if (_socket.nPutBytes >= nLen)
                               {
                                   _socket.nPutBytes = 0;
                                   _socket.packet = null;
                               }
                               else
                                   return false;
                           }
                           else if (ret <= 0)
                           {
                               if (sError != SocketError.WouldBlock)
                               {
                                   _socket.Disconnect();
                                   return false;
                               }
                               else
                               {
                                   return false;
                               }
                           }
                       }
                       catch (Exception e)
                       {
                           MyConsole.SaveException(e);
                           _socket.Disconnect();
                           return false;
                       }
                   }
                   //else
                    //   return;
               }

               //int counts = 200;
           //    while (counts > 0 /*&& WindowsAPI.ws2_32.CanWrite(_socket)*/ && _socket.CanDequeue(out _socket.packet))
               if(_socket.CanDequeue(out _socket.packet))
               {
                //   counts--;
                   try
                   {

                       if (!_socket.Alive || _socket.packet == null)
                           return false;
                           int nLen = _socket.packet.Length;
                           ret = _socket.Connection.Send(_socket.packet, _socket.nPutBytes, nLen - _socket.nPutBytes, SocketFlags.None, out sError);
                           if (ret > 0)
                           {
                               _socket.nPutBytes += ret;//Connection are NonBlocking = true? yes
                               if (_socket.nPutBytes < _socket.packet.Length)
                                   return false;
                               else
                               {
                                   _socket.packet = null;
                                   _socket.nPutBytes = 0;
                                   return true;
                               }
                           }
                           else
                           {
                               if (sError != SocketError.WouldBlock)
                               {
                                   _socket.Disconnect();
                                   return false;
                               }
                               else
                                   return false;

                           }
                       
                   }
                   catch (Exception e)
                   {
                       MyConsole.WriteException(e);
                       _socket.Disconnect();
                       return false;
                   }
               }
       
           }
           return false;
       }

       public object SynRoot = new object();
        public void Disconnect()
        {
            lock (SynRoot)
            {
                if (Alive)
                {
                   
                        OnSend.Clear();
                        if (Server != null)
                            Server.Clients.Remove(this);

                    Alive = false;
                    if (TimerSubscriptions != null)
                    {
                        for (int i = 0; i < TimerSubscriptions.Length; i++)
                            if (TimerSubscriptions[i] != null)
                                TimerSubscriptions[i].Dispose();
                    }
                    try
                    {
                      
                        WindowsAPI.ws2_32.shutdown(Connection.Handle, WindowsAPI.ws2_32.ShutDownFlags.SD_BOTH);
                        WindowsAPI.ws2_32.closesocket(Connection.Handle);
                        Connection.Dispose();
                        GC.SuppressFinalize(Connection);
                    }
                    catch(Exception e)
                    {
                        MyConsole.SaveException(e);
                    }
                    finally
                    {
                        if (OnDisconnect != null)
                            OnDisconnect.Invoke(this);
                    }
                }
            }
        }
    }
}
