using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Project_Terror_v2.Game.MsgServer;
using Project_Terror_v2.Game;
using Project_Terror_v2.Cryptography;
using ProtoBuf;
using Project_Terror_v2.Database;
using System.Net;


namespace Project_Terror_v2
{
    using PacketInvoker = CachedAttributeInvocation<Action<Client.GameClient, ServerSockets.Packet>, PacketAttribute, ushort>;

    class Program
    {
        public static List<byte[]> LoadPackets = new List<byte[]>();
        public static ServerSockets.SocketThread SocketsGroup;
        public static List<uint> ProtectMapSpells = new List<uint>() { 1038, Game.MsgTournaments.MsgSuperGuildWar.MapID };
        public static List<uint> MapCounterHits = new List<uint>() { 1005, 6000 };
        public static bool OnMainternance = false;
     
        public static Extensions.Time32 SaveDBStamp = Extensions.Time32.Now.AddMilliseconds(KernelThread.SaveDatabaseStamp);

        public static List<uint> NoDropItems = new List<uint>() { 1764, 700, 3954, 3820 };
        public static List<uint> FreePkMap = new List<uint>() { 3998,3071, 6000, 6001,1505, 1005, 1038, 700,1508/*PkWar*/, Game.MsgTournaments.MsgSuperGuildWar.MapID, Game.MsgTournaments.MsgCaptureTheFlag.MapID
        , Game.MsgTournaments.MsgTeamDeathMatch.MapID };
        public static List<uint> BlockAttackMap = new List<uint>() { 3825,3830, 3831, 3832,3834,3826,3827,3828,3829,3833, 9995,1068, 4020, 4000, 4003, 4006, 4008, 4009 , 1860 ,1858, 1801, 1780, 1779/*Ghost Map*/, 9972, 1806, 1002, 3954, 3081, 1036, 1004, 1008, 601, 1006, 1511, 1039, 700, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, (uint)Game.MsgTournaments.MsgSteedRace.Maps.DungeonRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.IceRace
        ,(uint)Game.MsgTournaments.MsgSteedRace.Maps.IslandRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.LavaRace, (uint)Game.MsgTournaments.MsgSteedRace.Maps.MarketRace};
        public static List<uint> BlockTeleportMap = new List<uint>() {  601, 6000, 6001, 1005, 700, 1858, 1860, 3852, Game.MsgTournaments.MsgEliteGroup.WaitingAreaID, 1768 };
        public static Role.Instance.Nobility.NobilityRanking NobilityRanking = new Role.Instance.Nobility.NobilityRanking();
        public static Role.Instance.ChiRank ChiRanking = new Role.Instance.ChiRank();
        public static Role.Instance.Flowers.FlowersRankingToday FlowersRankToday = new Role.Instance.Flowers.FlowersRankingToday();
        public static Role.Instance.Flowers.FlowerRanking GirlsFlowersRanking = new Role.Instance.Flowers.FlowerRanking();
        public static Role.Instance.Flowers.FlowerRanking BoysFlowersRanking = new Role.Instance.Flowers.FlowerRanking(false);

        public static ShowChatItems GlobalItems;
        public static SendGlobalPacket SendGlobalPackets;
        public static PacketInvoker MsgInvoker;
        public static ServerSockets.ServerSocket GameServer;

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleHandlerDelegate handler, bool add);
        private delegate bool ConsoleHandlerDelegate(int type);
        private static ConsoleHandlerDelegate handlerKeepAlive;

        public  static bool ProcessConsoleEvent(int type)
        {
            try
            {
                if (ServerConfig.IsInterServer)
                {
                    foreach (var client in Database.Server.GamePoll.Values)
                    {
                        try
                        {
                            if (client.Socket != null)//for my fake accounts !
                                client.Socket.Disconnect();
                        }
                        catch (Exception e)
                        {
                            MyConsole.WriteLine(e.ToString());
                        }
                    }
                    return true;
                }
                try
                {
                    if (WebServer.Proces.AccServer != null)
                    {
                        WebServer.Proces.Close();
                        WebServer.Proces.AccServer.Close();
                    }
                    if (GameServer != null)
                        GameServer.Close();

                   
                }
              catch (Exception e) { MyConsole.SaveException(e); }

                try
                {
                    foreach (var user in WebServer.LoaderServer.Clients.Values)
                        user.Disconnect();
                }
                catch(Exception e)
                {
                    MyConsole.SaveException(e);
                }
                MyConsole.WriteLine("Saving Database...");
          

                foreach (var client in Database.Server.GamePoll.Values)
                {
                    try
                    {
                        if (client.Socket != null)//for my fake accounts !
                            client.Socket.Disconnect();
                    }
                    catch (Exception e)
                    {
                        MyConsole.WriteLine(e.ToString());
                    }
                }
                Role.Instance.Clan.ProcessChangeNames();

                Database.Server.SaveDatabase();
                if (Database.ServerDatabase.LoginQueue.Finish())
                {
                    System.Threading.Thread.Sleep(1000);
                    MyConsole.WriteLine("Database Save Succefull.");
                }
            }
            catch (Exception e)
            {
                MyConsole.SaveException(e);
            }
            return true;
        }

          public static Extensions.Time32 ResetRandom = new Extensions.Time32();

          public static Extensions.SafeRandom GetRandom = new Extensions.SafeRandom();
        public static Extensions.RandomLite LiteRandom = new Extensions.RandomLite();

        public static class ServerConfig
        {
            public static string CO2Folder = "";
#if Encore
           public static string XtremeTopLink = "http://www.xtremetop100.com/in.php?site=1132352308&postback=";
            public static string Chatbox = "http://www.forum.ourconquer.com";
            public static string ChangePassword = "http://www.ourconquer.com/changepassword";
            public static string StorePage = "http://www.ourconquer.com/store";

#else
            public static string XtremeTopLink = "http://www.xtremetop100.com/in.php?site=1132355001";
#endif

            public static uint ServerID = 0;

            public static string IPAddres = "95.77.126.18";
            public static ushort AuthPort = 9960;
            public static ushort GamePort = 5816;
            public static string ServerName = "";

            public static string OfficialWebSite = "";
            //InternetPort
            public static ushort Port_BackLog;
            public static ushort Port_ReceiveSize = 8191;
            public static ushort Port_SendSize = 8191;

            //Database
            public static string DbLocation = "";

            //web Server
            public static ushort WebPort = 9900;
            public static string AccServerIPAddres = "";

            public static uint ExpRateSpell = 2;
            public static uint ExpRateProf = 2;
            public static uint UserExpRate = 9;
            public static uint PhysicalDamage = 100;// + 150%

            //loader
            public static string LoaderIP = "95.77.126.18";
            public static ushort LoaderPort = 9960;

            //interServer
            public static string InterServerAddress = "";
            public static ushort InterServerPort = 0;
            public static bool IsInterServer = false;
        }

        //You do not have 500 silvers with you.
        //Sorry, but you don`t have enough CPs.
        //Please come back when you will have 1 Star Crystal in your inventory.
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
     
       static int CutTrail(int x, int y) { return (x >= y) ? x : y; }
      static  int AdjustDefence(int nDef, int power2, int bless)
{
	int nAddDef = 0;
	nAddDef += Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(nDef, 100 -   power2, 100) - nDef;
   // nAddDef += Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(nDef, bless, 100);
          //nAddDef += Game.MsgServer.AttackHandler.Calculate.Base.AdjustDataEx(nDef,def2) - nDef;

    return Game.MsgServer.AttackHandler.Calculate.Base.MulDiv(nDef + nAddDef, 100 - power2, 100);

/*	pStatus = QueryStatus(STATUS_DEFENCE3);
	if (!pStatus)
		pStatus = QueryStatus(STATUS_DEFENCE2);
	if (!pStatus)
		pStatus = QueryStatus(STATUS_DEFENCE1);
	if(pStatus)
	{
		nAddDef += ::CutTrail(0, AdjustData(nDef, pStatus->GetPower())) - nDef;
#ifdef _DEBUG
		this->SendSysMsg(_TXTATR_NORMAL, "def adjust STATUS_DEFENCE+: %d", pStatus->GetPower());
#endif
	}*/



	return nDef + nAddDef;
}


      /*# The base (at 0 defense) difference between attack and defense needed to add/subtract 50% damage
base_d_factor = 10
    
# Amount added to the base factor for every point of defense over 0
scaled_d_factor = 0.5

# ...(stuff goes here)

dif = attack - defense
if dif:
  sign_dif = sign(dif)
  scale = 1.0 + (-1.0/(sign_dif + dif/(base_d_factor + defense*scaled_d_factor)) + sign_dif)
  return attack * scale
else:  
      # else we'd be dividing by 0
  return attack*/


      public static void TESTT()
      {
          double base_d_factor = 130;
          double scaled_d_factor = 0.5;
          double dif = 139500 - 25000;
         double sign_dif =Math.Sign(dif);
         double scale = 1.0 + (-1.0 / (sign_dif + dif / (base_d_factor + 25000 * scaled_d_factor)) + sign_dif);
         double ttt = 139500 * scale;
      }


      public class sorine
      {
          public uint uid = 333;
      }


      static byte[] DecryptString(char[] str)
      {
          int i = 0;
          byte[] nstr = new byte[1000];
          do
          {
              nstr[i] = Convert.ToByte(str[i + 1] ^ 0x34);
          } while (nstr[i++] != 0);
          return nstr;
      }
      public static void writetext(string tes99)
      {
          char[] tg = new char[tes99.Length];
          for (int x = 0; x < tes99.Length; x++)
              tg[x] = tes99[x];
          var hhhh = DecryptString(tg);
          Console.WriteLine(ASCIIEncoding.ASCII.GetString(hhhh));
      }
      public static int n, sol;
      public static int[] v = new int[100];
        public static void afisare()
      {
          Console.WriteLine("");
          int i, j, x;
          sol++;

          Console.WriteLine("sol: " + sol);
          for (i = 1; i <= n; i++)
          {
              Console.Write(v[i] + " ");
             /* for (j = 1; j <= n; j++)
              
                  /*if (v[i] == j)
                      Console.Write("D ");
                  else
                      Console.Write("_ ");
              Console.Write(Environment.NewLine);*/
          }
          Console.Write(Environment.NewLine);
      }
      public static int valid(int k)
      {
          int i;
          for (i = 1; i <= k - 1; i++)
              if ((v[k] <= v[i]))//|| (Math.Abs(v[k] - v[i]) == (k - i)))
                  return 0;
          return 1;
      }
      public static int solutie(int k)
      {
          if (k == n)
              return 1;
          return 0;
      }
      public static void BK(int k)
      {
          for (int i = 1; i <= n; i++)
          {
              v[k] = i;
              if (valid(k) == 1)
              {
                  if (solutie(k) == 1)
                      afisare();
                  else
                      BK(k + 1);
              }
          }
      } 
        public static unsafe void Main(string[] args)
        {



           

        /*
writetext("\x14\x5F\x5D\x40\x40\x4D\x5F\x55\x40\x47\x1A\x4E\x55\x44\x40\x5B\x1A\x5B\x46\x53\x34");
writetext("\x16\x46\x5B\x58\x58\x55\x57\x5B\x55\x47\x40\x55\x1A\x4E\x55\x44\x40\x5B\x1A\x5B\x46\x53\x34");
writetext("\x15\x50\x55\x47\x57\x55\x46\x4D\x56\x5B\x4D\x1A\x4E\x55\x44\x40\x5B\x1A\x5B\x46\x53\x34");

writetext("\x7\x63\x5D\x5A\x55\x59\x44\x34");
writetext("\xB\x43\x5D\x5A\x55\x59\x44\x1A\x51\x4C\x51\x34");
writetext("\x10\x57\x59\x50\x14\x1B\x57\x14\x47\x40\x55\x46\x40\x14\x11\x47\x34");
writetext("\x21\x73\x71\x60\x14\x11\x47\x14\x7C\x60\x60\x64\x1B\x5\x1A\x5\x39\x3E\x7C\x5B\x47\x40\xE\x14\x11\x47\xE\x11\x50\x39\x3E\x39\x3E\x34");

writetext("\x2E\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x34");
writetext("\x32\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x7B\x5A\x57\x51\x34");
writetext("\x36\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x67\x51\x46\x42\x5D\x57\x51\x47\x34");
writetext("\x3A\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x67\x51\x46\x42\x5D\x57\x51\x47\x7B\x5A\x57\x51\x34");
writetext("\x40\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x64\x5B\x58\x5D\x57\x5D\x51\x47\x68\x71\x4C\x44\x58\x5B\x46\x51\x46\x68\x66\x41\x5A\x34");
writetext("\x3A\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x14\x7A\x60\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x58\x5B\x55\x50\x34");

writetext("\x2E\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x34");
writetext("\x32\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x7B\x5A\x57\x51\x34");
writetext("\x34\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x7B\x5A\x57\x51\x71\x4C\x34");
writetext("\x36\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x67\x51\x46\x42\x5D\x57\x51\x47\x34");
writetext("\x3A\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x67\x51\x46\x42\x5D\x57\x51\x47\x7B\x5A\x57\x51\x34");
writetext("\x38\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x66\x41\x5A\x7B\x5A\x57\x51\x68\x67\x51\x40\x41\x44\x34");
writetext("\x40\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x64\x5B\x58\x5D\x57\x5D\x51\x47\x68\x71\x4C\x44\x58\x5B\x46\x51\x46\x68\x66\x41\x5A\x34");
writetext("\x3F\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x14\x7A\x60\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x63\x5D\x5A\x58\x5B\x53\x5B\x5A\x68\x61\x47\x51\x46\x5D\x5A\x5D\x40\x34");
writetext("\x4A\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x67\x5C\x51\x58\x58\x67\x51\x46\x42\x5D\x57\x51\x7B\x56\x5E\x51\x57\x40\x70\x51\x58\x55\x4D\x78\x5B\x55\x50\x50\x5B\x43\x47\x34");
writetext("\x47\x67\x7B\x72\x60\x63\x75\x66\x71\x68\x79\x5D\x57\x46\x5B\x47\x5B\x52\x40\x68\x63\x5D\x5A\x50\x5B\x43\x47\x68\x77\x41\x46\x46\x51\x5A\x40\x62\x51\x46\x47\x5D\x5B\x5A\x68\x71\x4C\x44\x58\x5B\x46\x51\x46\x68\x67\x5C\x55\x46\x51\x50\x60\x55\x47\x5F\x67\x57\x5C\x51\x50\x41\x58\x51\x46\x34");
       */
         /*   int ticks = Environment.TickCount;
            int playeruid = 1000836;


            //4-> ticks
            //8-> uid inter servr
            //12-> 420576 -> next decrypt receive  (945748903)
            //16-> 24641962 -> next decrypt receive(958435565)

            int crypt = 420576;//ticks ^ (playeruid * playeruid + 9527);
                      
            int crypt2 = crypt ^ (playeruid * playeruid + 9527);
            */
          
            //1223022395

            //3999900001

            //1891453317
            //m_pInfo->dwData ^ (m_pInfo->idPlayer*m_pInfo->idPlayer+9527
            
                /*first time
attack = (leftweapon(just first status) / 2) + attack all items + strength;
attack += attack * gems / 100;
attack += attack * prof / 100;
attack -= defence;
attack += attack * spellpower / 100;


if(target.battlePower > attacker.battlepower || OnBreak())
attack -> calculation disdain
attack -= attack * min(50, supertortoisegems) / 100;
attack = attack * 7000 / 1000;//7000 = target 2nd rb
attack -= attack * bless / 100;*/

                //   
                /*range attack
attack * 84 / 100;
attack * powerspell 
attack * 12 / 100;
attack * 7000 /10000;*/

           /*
                  uint atest = 47;
                      atest -= 48;

                      const int Dodge = 98;
                     //all items , chi.. inner + rightweapon(just main attack / 2); + strength + weapons * prof level
                      int damage = 41155 +200 + 1166 + 1166 / 2 + 130;// 200 strength  - > (1166 + 1166 /2)(weapons proficienty * weapons damage)
                      damage = 40000;
                   //   damage += damage * 8 / 100;
                         

                                      int ajust = 30130;//30130 like FastBlade
                                      int dmg2 = Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)(damage), 210, 100);//210 SDG`s
                                  //    dmg2 += damage * 30 / 100; // stigma
                                      damage += dmg2;//Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)dmg2, 0, 100);//- %6 stgs



                                      int defence = 25000;

                                      damage -= defence;



                                      damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.AdjustDataEx(damage, ajust);
                                    
                                      damage = Database.Disdain.UserAttackUser(405, 405, damage);
                                 //     damage -= damage * 50 / 100;
                                      damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)damage, 7000, 10000);

                                      damage -= damage * 54 / 100;


                              //       damage = Database.Disdain.UserAttackUser(407, 408, damage);

                                     //if target is reborn

                                  /*    damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)damage, 7000, 10000);

                                      damage -= damage * 54 / 100;
                */
                                      //12175
                                      //12574
                                      //13373
                                      //14774
                                      //14897

                                    //  for (int x = 0; x < 50; x++)
                                      
                                       /*   int da = 166554;
                   
                                          da = Database.Disdain.UserAttackUser(406, 405, da);
                                          Console.WriteLine("--->"+da);
                                          da -= da * 84 / 100;

                                          da = (int)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)da, 7000, 10000);

                                          da -= da * 54 / 100;
                                          Console.WriteLine(da);*
                                      }
                                 //   if(target.battlepower > user.battlepower)
                //                     damage = (int)Game.MsgServer.AttackHandler.Calculate.Base.MulDiv((int)damage, 3000, 10000);
                
                                      //to do calculate talismans.
               
                            
                                      return;
                          
                */




            byte[] proto = new byte[]
            {
                0x0A ,0x06 ,0x08 ,0xBE ,0x56 ,0x10 ,0xAD ,0x2B
            };

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                stream.InitWriter();
                for (int x = 0; x < proto.Length; x++)
                    stream.Write((byte)proto[x]);
                stream.Finalize(1153);
                MsgMagicColdTime.MagicColdTime obj = new MsgMagicColdTime.MagicColdTime();
              obj =  stream.ProtoBufferDeserialize<MsgMagicColdTime.MagicColdTime>(obj, proto);


            }
     //       return;
                                          try
                                          {
     

                MyConsole.DissableButton();
                
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                ServerSockets.Packet.SealString = "TQServer";



                Console.ForegroundColor = ConsoleColor.White;
                MyConsole.WriteLine("--------- Project_Terror_v2 -----------");
                MyConsole.WriteLine("This source was writen by Ceausu Sorin.");
                MyConsole.WriteLine("---------------------------------------\n");

              

                MsgInvoker = new PacketInvoker(PacketAttribute.Translator);
                Cryptography.DHKeyExchange.KeyExchange.CreateKeys();

                Game.MsgTournaments.MsgSchedules.Create();

                Database.Server.Initialize();

                SendGlobalPackets = new SendGlobalPacket();

                Cryptography.AuthCryptography.PrepareAuthCryptography();

                Database.Server.LoadDatabase();

                handlerKeepAlive = ProcessConsoleEvent;
                SetConsoleCtrlHandler(handlerKeepAlive, true);


               

                WebServer.LoaderServer.Init();
                WebServer.Proces.Init();

                if (ServerConfig.IsInterServer == false)
                {
                    GameServer = new ServerSockets.ServerSocket(
                        new Action<ServerSockets.SecuritySocket>(p => new Client.GameClient(p))
                        , Game_Receive, Game_Disconnect);
                    GameServer.Initilize(ServerConfig.Port_SendSize, ServerConfig.Port_ReceiveSize, 1, 3);
                    GameServer.Open(ServerConfig.IPAddres, ServerConfig.GamePort, ServerConfig.Port_BackLog);
            
                }


                GlobalItems = new ShowChatItems();

                Database.NpcServer.LoadServerTraps();

                MsgInterServer.PipeServer.Initialize();

                ThreadPoll.Create();


           //     ServerSockets.SocketPoll.Create("ConquerSockets");

                SocketsGroup = new ServerSockets.SocketThread("ConquerServer"
                    , GameServer
                    , MsgInterServer.PipeServer.Server
                    , WebServer.LoaderServer.Server
                    , WebServer.Proces.AccServer);

                SocketsGroup.Start();

                MsgInterServer.StaticConnexion.Create();

                Game.MsgTournaments.MsgSchedules.ClanWar = new Game.MsgTournaments.MsgClanWar();


              
                new KernelThread(300, "ConquerServer2").Start();
                new MapGroupThread(100, "ConquerServer3").Start();
                
                //    Database.ServerDatabase.Testtt();

            }
            catch (Exception e) { MyConsole.WriteException(e); }

            for (; ; )
                ConsoleCMD(MyConsole.ReadLine());
        }

        public static void SaveDBPayers(Extensions.Time32 clock)
        {

            if (clock > SaveDBStamp)
            {
                if (Database.Server.FullLoading && !Program.ServerConfig.IsInterServer)
                {
                    foreach (var user in Database.Server.GamePoll.Values)
                    {
                        if (user.OnInterServer)
                            continue;
                        if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            user.ClientFlag |= Client.ServerFlag.QueuesSave;
                            Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                        }
                    }
                    Database.Server.SaveDatabase();
                    MyConsole.WriteLine("Database got saved ! ");
                }
                SaveDBStamp.Value = clock.Value + KernelThread.SaveDatabaseStamp;
            }

        }
        public unsafe static void ConsoleCMD(string cmd)
        {
            try
            {
                string[] line = cmd.Split(' ');

                switch (line[0])
                {
                    case "powerarena":
                        {
                            Game.MsgTournaments.MsgSchedules.PowerArena.Start();
                            break;
                        }
                    case "squidward":
                        {
                            Game.MsgTournaments.MsgSchedules.SquidwardOctopus.Start();
                            break;
                        }
                    case "save":
                        {
                            Database.Server.SaveDatabase();
                            if (Database.Server.FullLoading && !Program.ServerConfig.IsInterServer)
                            {
                                foreach (var user in Database.Server.GamePoll.Values)
                                {
                                    if (user.OnInterServer)
                                        continue;
                                    if ((user.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                                    {
                                        user.ClientFlag |= Client.ServerFlag.QueuesSave;
                                        Database.ServerDatabase.LoginQueue.TryEnqueue(user);
                                    }
                                }
                                MyConsole.WriteLine("Database got saved ! ");
                            }
                            if (Database.ServerDatabase.LoginQueue.Finish())
                            {
                                System.Threading.Thread.Sleep(1000);
                                MyConsole.WriteLine("Database saved successfully.");
                            }
                            break;
                        }
              
                    case "steed":
                        {
                            Game.MsgTournaments.MsgSchedules.SteedRace.Create();
                            break;
                        }
                    case "ctfon":
                        {
                            Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Start();
                            break;
                        }
                    case "kick":
                        {

                            foreach (var user in Database.Server.GamePoll.Values)
                            {
                                if (user.Player.Name.Contains(line[1]))
                                {
                                    user.EndQualifier();
                                }
                            }
                            break;
                        }

                    case "pk":
                        {
                            Game.MsgTournaments.MsgSchedules.ElitePkTournament.Start();

                            foreach (var clients in Database.Server.GamePoll.Values)
                            {
                                Game.MsgTournaments.MsgSchedules.ElitePkTournament.SignUp(clients);
                            }
                            break;
                        }
                    case "teampk":
                        {
                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.Start();
                            var array = Database.Server.GamePoll.Values.ToArray();


                            for (int x = 0; x < array.Length - 5; x += 5)
                            {
                                if (array[x].Team == null)
                                {
                                    try
                                    {
                                        array[x].Team = new Role.Instance.Team(array[x]);
                                        Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x]);
                                        using (var rec = new ServerSockets.RecycledPacket())
                                        {
                                            var stream = rec.GetStream();
                                            array[x + 1].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 1]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 1]);

                                            array[x + 2].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 2]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 2]);

                                            array[x + 3].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 3]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 3]);

                                            array[x + 4].Team = array[0].Team;
                                            array[x].Team.Add(stream, array[x + 4]);
                                            Game.MsgTournaments.MsgSchedules.SkillTeamPkTournament.SignUp(array[x + 4]);
                                        }

                                    }
                                    catch { }
                                }
                            }
                            break;
                        }
                    case "search":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;
                        
                                string Name = ini.ReadString("Character", "Name", "None");
                                if (Name.ToLower() == line[1].ToLower() || Name.Contains(line[1]))
                                {
                                    Console.WriteLine(ini.ReadUInt32("Character", "UID", 0));
                                    break;
                                }
                               
                            }
                            break;
                        }
                    case "resetnobility":
                        {
                            WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                            foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                            {
                                ini.FileName = fname;

                                ulong nobility = ini.ReadUInt64("Character", "DonationNobility", 0);
                                nobility = nobility * 30 / 100;
                                ini.Write<ulong>("Character", "DonationNobility", nobility);
                            }

                            break;
                        }
                    case "check":
                        {
                              WindowsAPI.IniFile ini = new WindowsAPI.IniFile("");
                              foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Users\\"))
                              {
                                  ini.FileName = fname;

                                  long nobility = ini.ReadInt64("Character", "Money", 0);
                                  if (nobility < 0)
                                  {
                                      Console.WriteLine("");
                                  }

                              }
                            break;
                        }
                    case "fixedgamemap":
                        {
                            Dictionary<int, string> maps = new Dictionary<int, string>();
                            using (var gamemap = new BinaryReader(new FileStream(Path.Combine(Program.ServerConfig.CO2Folder, "ini/gamemap.dat"), FileMode.Open)))
                            {

                                var amount = gamemap.ReadInt32();
                                for (var i = 0; i < amount; i++)
                                {

                                    var id = gamemap.ReadInt32();
                                    var fileName = Encoding.ASCII.GetString(gamemap.ReadBytes(gamemap.ReadInt32()));
                                    var puzzleSize = gamemap.ReadInt32();
                                    if (id == 1017)
                                    {
                                        Console.WriteLine(puzzleSize);
                                    }
                                    if (!maps.ContainsKey(id))
                                        maps.Add(id, fileName);
                                    else 
                                        maps[id] = fileName;
                                }
                            }
                            break;
                        }
                 

                    case "startgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Alive;
                            Game.MsgTournaments.MsgSchedules.GuildWar.Start();
                            break;
                        }
                    case "startsgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Start();
                            break;
                        }
                    case "finishsgw":
                        {
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.SuperGuildWar.CompleteEndGuildWar();
                            break;
                        }
                    case "finishgw":
                        {
                            Game.MsgTournaments.MsgSchedules.GuildWar.Proces = Game.MsgTournaments.ProcesType.Dead;
                            Game.MsgTournaments.MsgSchedules.GuildWar.CompleteEndGuildWar();
                            break;
                        }
                  
                    case "exit":
                        {
                            new Thread(new ThreadStart(Maintenance)).Start();
                            break;
                        }
                    case "forceexit":
                        {
                            ProcessConsoleEvent(0);

                            Environment.Exit(0);
                            break;
                        }
                    case "restart":
                        {
                            ProcessConsoleEvent(0);

                            System.Diagnostics.Process hproces = new System.Diagnostics.Process();
                            hproces.StartInfo.FileName = "Project_Terror_v2.exe";
                            hproces.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                            hproces.Start();

                            Environment.Exit(0);

                            break;
                        }
                  
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        public static void Maintenance()
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                OnMainternance = true;
                MyConsole.WriteLine("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 5minute0second. Please exitthe game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (5 Minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 4minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
               
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 4minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (4 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                       MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 3minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
         
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 3minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (3 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                  MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 2minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                        MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 2minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
         
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (2 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                   MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 1minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
             
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                 MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 1minute0second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
               
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (1 Minutes & 00 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 30);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                MyConsole.WriteLine("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.");
#if Arabic
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in 0minute30second. Please exit the game now.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
                
#else
                MsgMessage msg = new MsgMessage("The server will be brought down for maintenance in (0 Minutes & 30 Seconds). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 20);
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
#if Arabic
                  MsgMessage msg = new MsgMessage("Server maintenance(2 minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);
              
#else
                MsgMessage msg = new MsgMessage("Server maintenance(few minutes). Please log off immediately to avoid data loss.", "ALLUSERS", "GM", MsgMessage.MsgColor.red, MsgMessage.ChatMode.Center);

#endif
                SendGlobalPackets.Enqueue(msg.GetArray(stream));
            }
            Thread.Sleep(1000 * 10);
            ProcessConsoleEvent(0);

            Environment.Exit(0);
        }

        public unsafe static void Game_Receive(ServerSockets.SecuritySocket obj, ServerSockets.Packet stream)//ServerSockets.Packet data)
        {
            if (!obj.SetDHKey)
                CreateDHKey(obj, stream);
            else
            {
                try
                {
                    if (obj.Game == null)
                        return;
                     ushort PacketID = stream.ReadUInt16();
                  
                     if (obj.Game.Player.CheckTransfer)
                         goto jmp;
                    if (obj.Game.PipeClient != null && PacketID != Game.GamePackets.Achievement)
                    {
                        if (PacketID == (ushort)Game.GamePackets.MsgOsShop
             || PacketID == (ushort)Game.GamePackets.SecondaryPassword
                      || PacketID >= (ushort)Game.GamePackets.LeagueOpt && PacketID <= (ushort)Game.GamePackets.LeagueConcubines
                      || PacketID == (ushort)Game.GamePackets.LeagueRobOpt)
                            goto jmp;

                        stream.Seek(stream.Size);
                        obj.Game.PipeClient.Send(stream);

                        if (PacketID != 1009)
                        {
                           
                            return;
                        }
                        stream.Seek(4);
                    }
                   
#if TEST
                    MyConsole.WriteLine("Receive -> PacketID: " + PacketID);
#endif

                 //   Database.ServerDatabase.LoginQueue.Enqueue("[CallStack]" + MyConsole.log1(obj.Game.Player.Name, stream.Memory, stream.Size));
                    jmp:
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {
                        hinvoker(obj.Game, stream);
                    }
                    else
                    {
#if TEST
                        MyConsole.WriteLine("Not found the packet ----> " + PacketID);
#endif
                    }

                }
                catch (Exception e) { MyConsole.WriteException(e); }
                finally
                {
                    ServerSockets.PacketRecycle.Reuse(stream);
                }
            }

        }
        public unsafe static void CreateDHKey(ServerSockets.SecuritySocket obj, ServerSockets.Packet Stream)
        {
            try
            {
                byte[] buffer = new byte[36];
                bool extra = false;
                string text = System.Text.ASCIIEncoding.ASCII.GetString(obj.DHKeyBuffer.buffer, 0, obj.DHKeyBuffer.Length());
                if (!text.EndsWith("TQClient"))
                {
                    System.Buffer.BlockCopy(obj.EncryptedDHKeyBuffer.buffer, obj.EncryptedDHKeyBuffer.Length() - 36, buffer, 0, 36);
                    extra = true;
                }
//                MyConsole.PrintPacketAdvanced(Stream.Memory, Stream.Size);

                string key;
                if (Stream.GetHandshakeReplyKey(out key))
                {
                    obj.SetDHKey = true;
                    obj.Game.DHKey.HandleResponse(key);
                    var compute_key = obj.Game.DHKeyExchance.PostProcessDHKey(obj.Game.DHKey.ToBytes());
                    //obj.Game.Crypto.SetIVs(new byte[8], new byte[8]);
                    obj.Game.Crypto.GenerateKey(compute_key);
                    obj.Game.Crypto.Reset();
                }
                else
                {
                    obj.Disconnect();
                    return;
                }
                if (extra)
                {

                    Stream.Seek(0);
                    obj.Game.Crypto.Decrypt(buffer, 0, Stream.Memory, 0, 36);
                    Stream.Size = buffer.Length;
                 

                  
               

                    Stream.Size = buffer.Length;
                    Stream.Seek(2);
                    ushort PacketID = Stream.ReadUInt16();
                    Action<Client.GameClient, ServerSockets.Packet> hinvoker;
                    if (MsgInvoker.TryGetInvoker(PacketID, out hinvoker))
                    {
                        hinvoker(obj.Game, Stream);
                    }
                    else
                    {
                        obj.Disconnect();

                        MyConsole.WriteLine("DH KEY Not found the packet ----> " + PacketID);

                    }
                }
                
            }
            catch (Exception e) { MyConsole.WriteException(e); }
        }
        public unsafe static void Game_Disconnect(ServerSockets.SecuritySocket obj)
        {
           
            if (obj.Game != null && obj.Game.Player != null)
            {
                try
                {
                    Console.WriteLine("[" + obj.Game.Player.UID + "-" + obj.Game.ClientFlag + "] " + obj.Game.Player.Name + " Try to disconnect");
                    Client.GameClient client;
                    if (Database.Server.GamePoll.TryGetValue(obj.Game.Player.UID, out client))
                    {
                        if (client.OnInterServer)
                            return;
                        if ((client.ClientFlag & Client.ServerFlag.LoginFull) == Client.ServerFlag.LoginFull)
                        {
                            if (obj.Game.PipeClient != null)
                                obj.Game.PipeClient.Disconnect();
                            MyConsole.WriteLine("Client " + client.Player.Name + " was loggin out.");
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();

                                try
                                {
                                    if (client.Player.InUnion)
                                    {
                                        client.Player.UnionMemeber.Owner = null;
                                    }
                                    client.CheckRouletteDisconnect();
                                    client.EndQualifier();

                                    if (client.MyPokerTable != null)
                                    {
                                        client.MyPokerTable.TableMatch.DisconnetPlayer(client);
                                    }


                                    if (client.Team != null)
                                        client.Team.Remove(client, true);
                                    if (client.Player.MyClanMember != null)
                                        client.Player.MyClanMember.Online = false;
                                    if (client.IsVendor)
                                        client.MyVendor.StopVending(stream);
                                    if (client.InTrade)
                                        client.MyTrade.CloseTrade();
                                    if (client.Player.MyGuildMember != null)
                                        client.Player.MyGuildMember.IsOnline = false;

                                    if (client.Player.ObjInteraction != null)
                                    {
                                        client.Player.InteractionEffect.AtkType = Game.MsgServer.MsgAttackPacket.AttackID.InteractionStopEffect;

                                        InteractQuery action = InteractQuery.ShallowCopy(client.Player.InteractionEffect);

                                        client.Send(stream.InteractionCreate(&action));

                                        client.Player.ObjInteraction.Player.OnInteractionEffect = false;
                                        client.Player.ObjInteraction.Player.ObjInteraction = null;
                                    }


                                    client.Player.View.Clear(stream);


                                }
                                catch (Exception e)
                                {
                                    MyConsole.WriteException(e);
                                    client.Player.View.Clear(stream);
                                }
                                finally
                                {
                                    client.ClientFlag &= ~Client.ServerFlag.LoginFull;
                                    client.ClientFlag |= Client.ServerFlag.Disconnect;
                                    client.ClientFlag |= Client.ServerFlag.QueuesSave;
                                    Database.ServerDatabase.LoginQueue.TryEnqueue(client);
                                }

                                try
                                {
                                    client.Player.Associate.OnDisconnect(stream, client);

                                    //remove mentor and apprentice
                                    if (client.Player.MyMentor != null)
                                    {
                                        Client.GameClient me;
                                        client.Player.MyMentor.OnlineApprentice.TryRemove(client.Player.UID, out me);
                                        client.Player.MyMentor = null;
                                    }
                                    client.Player.Associate.Online = false;
                                    lock (client.Player.Associate.MyClient)
                                        client.Player.Associate.MyClient = null;
                                    foreach (var clien in client.Player.Associate.OnlineApprentice.Values)
                                        clien.Player.SetMentorBattlePowers(0, 0);
                                    client.Player.Associate.OnlineApprentice.Clear();
                                    //done remove
                                }
                                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
                            }
                        }
                    }
                }
                catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
            }
            else if (obj.Game != null)
            {
                if (obj.Game.ConnectionUID != 0)
                {
                    Client.GameClient client;
                    Database.Server.GamePoll.TryRemove(obj.Game.ConnectionUID, out client);
                }
            }
        }


        public static bool NameStrCheck(string name, bool ExceptedSize = true)
        {
            if (name == null)
                return false;
            if (name == "")
                return false;
            string ValidChars = "[^A-Za-z0-9ء-ي*~.&.$]$";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(ValidChars);
            if (r.IsMatch(name))
                return false;
         
            if (name.Contains('/'))
                return false;
            if (name.Contains(@"\"))
                return false;
            if (name.Contains(@"'"))
                return false;
        //    if (name.Contains('#'))
          //      return false;
            if (name.Contains("GM") ||
                name.Contains("PM") ||
                name.Contains("SYSTEM") ||
                name.Contains("{") || name.Contains("}") || name.Contains("[") || name.Contains("]"))
                return false;
            if (name.Length > 16 && ExceptedSize)
                return false;
            for (int x = 0; x < name.Length; x++)
                if (name[x] == 25)
                    return false;
            return true;
        }
        public static bool StringCheck(string pszString)
        {
            for (int x = 0; x < pszString.Length; x++)
            {
                if (pszString[x] > ' ' && pszString[x] <= '~')
                    return false;
            }
            return true;
        }
    }
}
