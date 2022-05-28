using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMUpdateAlive : NMEvent
    {
        public int _hunters = -1;
        public int _hiders = -1;

        public NMUpdateAlive(int hunters, int hiders)
        {
            _hunters = hunters;
            _hiders = hiders;
        }

        public NMUpdateAlive()
        {
        }

        public override void Activate()
        {
            if(PropHunt.core.Data != null) PropHunt.core.Data.UpdateAliveDucks(_hiders,_hunters);
        }
    }
}
