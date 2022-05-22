using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|tool")]
    public class PHHiderTool : PHTool
    {
        public float _checkRadius = 7f;
        private Holdable _attachedProp;

        public PHHiderTool(float xval, float yval) : base(xval, yval)
        {

        }

        public override void Fire()
        {
            if (_attachedProp != null) UnDisguise();
            else if(_fireTimer <= 0f)
            {
                Holdable t = GetNearestProp();
                if (t != null)
                {
                    _fireTimer = fireCooldown;
                    if (Network.isActive)
                    {
                        Send.Message(new NMPop(duck.position));
                    }
                    NMPop.AmazingDisappearingParticles(duck.position);
                    DisguiseAsProp(t);
                }
            }
        }

        private Holdable GetNearestProp()
        {
            IEnumerable<Holdable> props = Level.CheckCircleAll<Holdable>(this.position, _checkRadius);
            foreach (Holdable p in props)
            {
                bool validProp = p != this && p.canPickUp && !(p is IAmADuck) && (p.equippedDuck == null);
                if (validProp) return p;
            }
            return null;
        }

        private void DisguiseAsProp(Holdable t)
        {
            equippedDuck.visible = false;
            _attachedProp = t;
            equippedDuck.Fondle(_attachedProp);
            if (_attachedProp.equippedDuck != null) _attachedProp.equippedDuck.Disarm(this.equippedDuck);
            _attachedProp.canPickUp = false;
            _attachedProp.enablePhysics = false;
        }

        private void UnDisguise()
        {
            equippedDuck.visible = true;
            if (_attachedProp != null)
            {
                _attachedProp.canPickUp = true;
                _attachedProp.enablePhysics = true;
                _attachedProp.ApplyForce(new Vec2(0,-1f));
                equippedDuck.Fondle(_attachedProp);
                if (_attachedProp.equippedDuck != null) _attachedProp.equippedDuck.Disarm(this.equippedDuck);
            }
            _attachedProp = null;
        }

        public override void DrawTeamMates(List<Duck> teamMates)
        {
            foreach (Duck d in teamMates)
            {
                Vec2 circlePosition;
                Equipment eq = d.GetEquipment(GetType());
                if (eq != null)
                {
                    PHHiderTool tool = (PHHiderTool)eq;
                    circlePosition = tool._attachedProp == null ? d.position : tool._attachedProp.position;
                }
                else
                {
                    circlePosition = d.position;
                }
                Graphics.DrawCircle(circlePosition, 6f, Color.Blue);
            }
        }

        public override void Update()
        {
            base.Update();
            if (_attachedProp != null && equippedDuck != null)
            {
                if (_attachedProp.destroyed) UnDisguise();
                else
                {
                    if (equippedDuck.visible) equippedDuck.visible = false;
                    _attachedProp.position.x = equippedDuck.position.x;
                    _attachedProp.position.y = equippedDuck.position.y + _attachedProp.height * 0.5f;
                }
            }

        }

        public override void Thrown()
        {
            base.Thrown();
            this.Presto();
        }

    }
}