using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.WebServer
{
    public class ConnectionPoll
    {

        public class Connection
        {
            public uint UID;
            public uint hash;
            public DateTime TimerJoin = new DateTime();

            public Connection(uint _uid, uint str)
            {
                hash = str;
                UID = _uid;
                TimerJoin = DateTime.Now;
            }
        }

        public Extensions.SafeDictionary<uint, Connection> Poll = new Extensions.SafeDictionary<uint, Connection>();


        public void Add(uint UID, uint hash)
        {
         
            if (Poll.ContainsKey(UID))
            {
                Poll[UID].UID = UID;
                Poll[UID].TimerJoin = DateTime.Now;
                Poll[UID].hash = hash;
            }
            else
            {
                Connection conn = new Connection(UID, hash);
                Poll.Add(conn.UID, conn);
            }
        }
        public bool CheckJoin(uint UID, uint hash)
        {

            Connection conn;
            if (Poll.TryGetValue(UID, out conn))
            {
                if (conn.hash == hash)
                {
                    if (conn.TimerJoin.AddSeconds(120) > DateTime.Now)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    
    }
}
