using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt")]
    [BaggedProperty("isOnlineCapable", true)]
    public class PHHunterTool : PHTool
    {
        public PHHunterTool(float xval, float yval) : base(xval, yval)
        {
            fireCooldown = 6f;
        }

        public override void Fire()
        {
            if (PropHunt.core.IsPHLevel)
            {
                if (PropHunt.core.Gamemode.Status != PHGameStatus.HUNTING)
                {
                    return;
                }
            }

            if (_fireTimer <= 0f)
            {
                _fireTimer = fireCooldown;
                if (Network.isActive)
                {
                    Send.Message(new NMPop(duck.position));
                }
                NMPop.AmazingDisappearingParticles(duck.position);
            }
        }

        public override void DrawTeamMates(List<Duck> teamMates)
        {
            foreach (Duck d in teamMates)
            {
                Graphics.DrawCircle(d.position, 6f, Color.DarkRed);
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