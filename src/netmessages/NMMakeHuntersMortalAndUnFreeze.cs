using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMMakeHuntersMortalAndUnFreeze : NMEvent
    {
        public Duck _duck;
        public NMMakeHuntersMortalAndUnFreeze(Duck d)
        {
            _duck = d;
        }
        public NMMakeHuntersMortalAndUnFreeze()
        {
        }

        public override void Activate()
        {
            _duck.moveLock = false;
            _duck.invincible = false;
        }
    }
}
