using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|tools")]
    public class PHHunterTool : PHTool
    {
        public float _checkRadius = 5f;

        public override Color TeamColor
        {
            get
            {
                return Color.Red;
            }
        }

        public PHHunterTool(float xval, float yval) : base(xval, yval)
        {
            fireCooldown = 3f;
            _sprite.color = TeamColor;
            _teamMarker.color = TeamColor;
            _health = 8;
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
                if (_target != null)
                {
                    _fireTimer = fireCooldown;
                    if (KillNearbyHiders())
                    {
                        DevConsole.Log("[PH] Hider killed");
                    }
                    else
                    {
                        _health--;
                        DevConsole.Log("[PH] Missed the hit, remaining HP: " + Health);
                        if(_health <= 0)
                        {
                            equippedDuck.Kill(new DTPop());
                        }
                    }
                }
            }
        }

        private bool KillNearbyHiders()
        {
            if (_target != null && _target is Duck)
            {
                ((Duck)_target).Kill(new DTIncinerate(this));
                return true;
            }
            else return false;
        }

        public override void PickTarget()
        {
            Duck d = Level.CheckCircle<Duck>(equippedDuck.position.x, equippedDuck.position.y, _checkRadius, equippedDuck);
            if (d != null && d.GetEquipment(typeof(PHHiderTool)) != null)
            {
                _target = d;
            }
            else
            {
                bool found = false;
                IEnumerable<Holdable> props = Level.CheckCircleAll<Holdable>(this.position, _checkRadius);
                foreach (Holdable p in props)
                {
                    bool validProp = p != this && p.canPickUp && !(p is IAmADuck) && (p.equippedDuck == null);
                    if (validProp)
                    {
                        _target = p;
                        found = true;
                        break;
                    }
                }
                if (!found) _target = null;
            }
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
            if (PropHunt.core.Data != null) _zoom = PropHunt.core.Data.HuntersZoom;
        }

        public override void Thrown()
        {
            base.Thrown();
            this.Presto();
        }

    }
}