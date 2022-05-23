using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|tools")]
    public class PHHunterTool : PHTool
    {
        float checkRadius = 5f;
        public PHHunterTool(float xval, float yval) : base(xval, yval)
        {
            fireCooldown = 6f;
        }

        public override void Fire()
        {
            if (PropHunt.core.IsPHLevel)
            {
                if (PropHunt.core.Data.Status != PHGameStatus.HUNTING)
                {
                    return;
                }
            }

            if (_fireTimer <= 0f)
            {
                _fireTimer = fireCooldown;
                KillNearbyHiders();
            }
        }

        private void KillNearbyHiders()
        {
            Duck d = Level.CheckCircle<Duck>(equippedDuck.position.x,equippedDuck.position.y,checkRadius,equippedDuck);
            if(d != null && d.GetEquipment(typeof(PHHiderTool)) != null)
            {
                if (Network.isActive)
                {
                    Send.Message(new NMPop(duck.position));
                }
                NMPop.AmazingDisappearingParticles(duck.position);
                d.Kill(new DTIncinerate(this));
            }
        }

        public override void DrawTeamMates(List<Duck> teamMates)
        {
            foreach (Duck d in teamMates)
            {
                Graphics.DrawCircle(d.position, 6f, Color.DarkRed);
            }
        }

        public override Vec2 CameraSize
        {
            get
            {
                if (PropHunt.core.Data.Status == PHGameStatus.HIDING) return new Vec2(1f,1f);
                else return base.CameraSize;
            }
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Thrown()
        {
            base.Thrown();
            this.Presto();
        }
    }
}