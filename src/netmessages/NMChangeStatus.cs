using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMChangeStatus : NMEvent
    {
        public int _status;
        public float _remaningTime;

        public NMChangeStatus(PHGameStatus status, float remaingTime)
        {
            _status = (int)status;
            _remaningTime = remaingTime;
        }

        public NMChangeStatus()
        {
            _remaningTime = -1f;
        }

        public override void Activate()
        {
            PropHunt.core.Data.ChangeStatus((PHGameStatus)_status, _remaningTime);
        }
    }
}
