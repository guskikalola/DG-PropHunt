using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMMakeHuntersInvincibleAndFreeze : NMEvent
    {
        public Duck _duck;
        public NMMakeHuntersInvincibleAndFreeze(Duck d)
        {
            _duck = d;
        }
        public NMMakeHuntersInvincibleAndFreeze()
        {
        }

        public override void Activate()
        {
            _duck.moveLock = true;
            _duck.invincible = true;
        }
    }
}
