using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMTaunt : NMEvent
    {
        public int _taunt;
        public byte _senderIndex;

        public NMTaunt(int taunt, byte senderIndex)
        {
            _taunt = taunt;
            _senderIndex = senderIndex;
        }

        public NMTaunt()
        {
        }

        public override void Activate()
        {
            Duck d = DuckNetwork.profiles[(int)_senderIndex].duck; 
            if(d != null)
            {
                if(!d.ShouldDrawIcon())
                {
                    if (_taunt < PropHunt.taunts.Count) PropHunt.taunts[_taunt].Play();
                }
            }
        }
    }
}
