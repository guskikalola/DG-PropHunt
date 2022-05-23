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
            fireCooldown = 3f;
            _sprite.color = Color.Red;
            _teamMarker.color = Color.Red;
        }

        public override void Fire()
        {
            base.Fire();
            if (PropHunt.core.IsPHLevel)
            {
                if (PropHunt.core.Data.Status != PHGameStatus.HUNTING)
                {
                    return;
                }
            }

            if (_fireTimer <= 0f)
            {
                _sprite.SetAnimation("active");
                if (KillNearbyHiders()) _fireTimer = fireCooldown;
            }
        }

        private bool KillNearbyHiders()
        {
            Duck d = Level.CheckCircle<Duck>(equippedDuck.position.x, equippedDuck.position.y, checkRadius, equippedDuck);
            if (d != null && d.GetEquipment(typeof(PHHiderTool)) != null)
            {
                d.Kill(new DTIncinerate(this));
                return true;
            }
            else return false;
        }

        public override void DrawTeamMates(List<Duck> teamMates)
        {
            foreach (Duck d in teamMates)
            {
                Graphics.Draw(_teamMarker, d.position.x, d.position.y - 10f);
            }
        }

        public override Vec2 CameraSize
        {
            get
            {
                if (PropHunt.core.Data.Status == PHGameStatus.HIDING) return new Vec2(1f, 1f);
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