
using System;

namespace DuckGame.PropHunt
{
    public class PHModeConfig
    {
        private float _hidingTime = 15f;
        private float _huntingTime = 30f;
        private int _huntersAmount = 1;
        public PHModeConfig()
        {

        }

        public float HidingTime { get => _hidingTime; set => _hidingTime = value; }
        public int HuntersAmount { get => _huntersAmount; set => _huntersAmount = value; }
        public float HuntingTime { get => _huntingTime; set => _huntingTime = value; }

        public override string ToString()
        {
            return String.Format("HidingTime: {0} | HuntingTime: {1} | HuntersAmount: {2}", HidingTime, HuntingTime, HuntersAmount);
        }
    }
}