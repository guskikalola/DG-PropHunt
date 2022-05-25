using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    public class PHData
    {
        private PHGameStatus _status = PHGameStatus.CREATED;
        private float _remainingTime = 0;
        private float _hidersZoom = 1;
        private float _huntersZoom = 1;
        private int _huntersAlive = 0;
        private int _hidersAlive = 0;
        public int winner = -1;

        private PHTool _tool;

        public int HidersAlive
        {
            get
            {
                return _hidersAlive;
            }
        }

        public int HuntersAlive
        {
            get
            {
                return _huntersAlive;
            }
        }

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

        public PHTool Tool
        {
            get
            {
                return _tool;
            }
            set
            {
                _tool = value;
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

        internal void UpdateAliveDucks(int hidersAlive, int huntersAlive)
        {
            _hidersAlive = hidersAlive;
            _huntersAlive = huntersAlive;
        }
    }
}
