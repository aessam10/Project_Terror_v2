using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_Terror_v2.Game.MsgTournaments
{
    public interface ITournament
    {
        ProcesType Process { get; set; }
        TournamentType Type { get; set; }
        void Open();
        bool Join(Client.GameClient user, ServerSockets.Packet stream);
        void CheckUp();
        bool InTournament(Client.GameClient user);
    }
}
