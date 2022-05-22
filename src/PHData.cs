using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    public class PHData
    {
        private PHGameStatus _status;
        private float _remainingTime;
        public int winner = -1;

        public PHGameStatus Status
        {
            get
            {
                return _status;
            }
        }
        public float RemainingTime
        {
            get
            {
                return _remainingTime;
            }
        }

        internal void ChangeStatus(PHGameStatus status, float remainingTime)
        {
            _status = status;
            _remainingTime = remainingTime;
        }

        public override string ToString()
        {
            return String.Format("Status: {0} | RemainingTime: {1}", Status, RemainingTime);
        }

        public void Update()
        {
            if(_remainingTime > 0f)
            {
                _remainingTime -= 0.01f;
            }
        }
    }
}
