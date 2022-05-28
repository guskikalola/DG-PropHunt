using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMRoundEnded : NMEvent
    {
        public int _winner;
        public NMRoundEnded(int winner)
        {
            _winner = winner;
        }

        public NMRoundEnded()
        {
        }

        public override void Activate()
        {
            if (PropHunt.core.Data != null) PropHunt.core.Data.winner = _winner;
        }
    }
}
