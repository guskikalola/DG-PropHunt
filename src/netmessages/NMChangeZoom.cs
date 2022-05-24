using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    class NMChangeZoom : NMEvent
    {
        public float _hidersZoom;
        public float _huntersZoom;

        public NMChangeZoom(float hidersZoom, float huntersZoom)
        {
            _hidersZoom = hidersZoom;
            _huntersZoom = huntersZoom;
        }

        public NMChangeZoom()
        {
        }

        public override void Activate()
        {
            if(PropHunt.core.Data != null) PropHunt.core.Data.ChangeZoom(_hidersZoom, _huntersZoom);
        }
    }
}
