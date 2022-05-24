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
        private float _hidersZoom;
        private float _huntersZoom;
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

        public float HuntersZoom
        {
            get
            {
                return _huntersZoom;
            }
        }        
        
        public float HidersZoom
        {
            get
            {
                return _hidersZoom;
            }
        }

        public void ChangeStatus(PHGameStatus status, float remainingTime)
        {
            _status = status;
            _remainingTime = remainingTime;
        }

        public void ChangeZoom(float hidersZoom, float huntersZoom)
        {
            _hidersZoom = hidersZoom;
            _huntersZoom = huntersZoom;
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
